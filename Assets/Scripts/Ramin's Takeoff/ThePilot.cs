using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using InControl;

public class ThePilot : AudioHandler {
    
    PilotAnimation _Animations;
    MeshRenderer planeRender;
    AdvanceScene advance;
    Rigidbody planeBody;

    [Header("Movement & Inputs")]
    public float moveSpeed;
    public float strafeSpeed = 125f;
    public float barrelRollForce = 5000f;
    public float maxVelocityZ = 125f;
    public float maxVelocityZfight = 95f;
    public float maxVelocityZspeedUp = 125f;
    public float maxVelocityXY = 100f;
    public float heightMin, heigtMax;
    public float xMin, xMax;
    //todo add velocity max for x/y and cap it at 666
    public float maxVelocityX = 666f;
    public float maxVelocityY = 666f;
    [SerializeField]
    private float xDecel = 0.8f;
    public bool controlsActive = true;
    public bool movementFrozen;
    public bool countingBullets;
    private bool barrelRoll;
    [Tooltip("For feeding plane velocity into the 0 - 1 animation value by division.")]
    [SerializeField]
    private float velocityAnimatorFactor = 100f;
    public float smoothTime = 0.5f;
    InputDevice inputDevice;

    //weapons 
    [Header("Weapons")]
    public Gun[] guns;
    [Tooltip("Time between bullets firing left")]
    public float weaponsTimerL, firingIntervalL = 0.05f;
    [Tooltip("Time between bullets firing right")]
    public float weaponsTimerR, firingIntervalR = 0.05f;
    [Tooltip("Controls UI which appears at start of sequence")] 
    public FadeUI[] weaponsControlFades;

    public bool useLockOnTargeting;
    [SerializeField]
    private Transform threeDTarget;
    private FollowPilot targetFollow;
    public Transform lockOnTarget;
    [Header("Pilot Views")]
    public bool zoomedIn;
    public GameObject fpCam,cockpit, zoCam;
    [Header("Cockpit UI")]
    public int bulletCount = 1800;
    public TMP_Text bText;
    [Header("Audio")]
    public AudioSource music;
    public AudioClip outOfAmmoClick;
    public AudioClip zapped;

    [Tooltip("We trigger this when the player runs out of bullets")]
    public EventTrigger triggerApocalypse;
  
    public override void Awake()
    {
        base.Awake();
        _Animations = GetComponentInChildren<PilotAnimation>();
        planeRender = _Animations.GetComponent<MeshRenderer>();
        advance = FindObjectOfType<AdvanceScene>();
        planeBody = GetComponent<Rigidbody>();
        guns = GetComponentsInChildren<Gun>();
        bText.text = bulletCount.ToString();
        SwitchViews(false);
        //set up targeting system
        targetFollow = threeDTarget.GetComponent<FollowPilot>();
    }
    
	//could play sounds when moving 
	void Update ()
    {
        if (controlsActive)
        {
            MovementInputs();
            
            CheckAnimations();

            FireWeapons();

            //switch views
            // if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)
            //     || inputDevice.DPadLeft.WasPressed || inputDevice.DPadRight.WasPressed
            //     || inputDevice.DPadUp.WasPressed || inputDevice.DPadDown.WasPressed)
            // {
            //     ToggleViews();
            // }
        }

        weaponsTimerL -= Time.deltaTime;
        weaponsTimerR -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (movementFrozen == false)
        {
            ApplyForces();

            Debug.Log("X velocity is " + planeBody.velocity.x);
            Debug.Log("Y velocity is " + planeBody.velocity.y);
        }
    }

    //resets min height 
    public void ResetMinHeight(float newHeight)
    {
        heightMin = newHeight;
    }

    //start the countdown
    public void StartCountingBullets()
    {
        countingBullets = true;
    }

    #region Camera Views
    public void ToggleViews()
    {
        zoomedIn = !zoomedIn;
        SwitchViews(zoomedIn);
    }

    public void SetFPView()
    {
        SwitchViews(true);
    }
    
    public void SetTPView()
    {
        SwitchViews(false);
    }

    private Coroutine camWait;
    void SwitchViews(bool fpORzoom)
    {
        //first person
        if (fpORzoom)
        {
            //change cams
            fpCam.SetActive(true);
            zoCam.SetActive(false);

            camWait = StartCoroutine(WaitToChangePlaneVisuals());
            //set animator 
            //_Animations.SetAnimator("idle");
            //_Animations.Animator.enabled = false;
            zoomedIn = true;
        }
        //zoomed out 
        else
        {
            //check for cam wait coroutine 
            if (camWait != null)
            {
                StopCoroutine(camWait);
                camWait = null;
            }
            
            //change cams
            fpCam.SetActive(false);
            zoCam.SetActive(true);
            
            //change plane visuals
            cockpit.SetActive(false);
            planeRender.enabled = true;
            
            //_Animations.Animator.enabled = true;
            zoomedIn = false;
        }
    }

    IEnumerator WaitToChangePlaneVisuals()
    {
        cockpit.SetActive(true);
        
        yield return new WaitForSeconds(1.75f);
        
        planeRender.enabled = false;
    }

    #endregion

    #region Weapons

    void FireWeapons()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //fire weapons inputs
        if (Input.GetMouseButton(0) || inputDevice.LeftTrigger || inputDevice.LeftBumper
         || Input.GetMouseButton(1) || inputDevice.RightTrigger || inputDevice.RightBumper)
        {
            //check can fire 
            if(weaponsTimerL < 0 || weaponsTimerR < 0)
            {
                //if we have bullets left 
                if(bulletCount > 1)
                {
                    //guns fire 
                    for (int i = 0; i < guns.Length; i++)
                    {
                        guns[i].SpawnBullet();
                    }

                    //only counting once deities appear 
                    if (countingBullets)
                    {
                        //lose a bullet for every gun fired 
                        bulletCount -= guns.Length;
                        //set bullet text 
                        bText.text = bulletCount.ToString();
                    }
                    
                    //weapons controls UI fade outs can only happen if they are already faded in.
                    if (weaponsControlFades[0].GetCurrentOpacity() > weaponsControlFades[0].fadeInAmount - 0.1f)
                    {
                        //fade out all weapons controls UIs 
                        for (int i = 0; i < weaponsControlFades.Length; i++)
                        {
                            weaponsControlFades[i].FadeOut();
                        }
                    }
                }
                //click click OUT OF AMMO - Trigger the apocalypse. 
                else
                {
                    PlaySoundRandomPitch(outOfAmmoClick, 1f);

                    if (!triggerApocalypse.hasTriggered)
                    {
                        //this will need to be replaced by transition 
                        triggerApocalypse.SetTrigger();
                    }
                }

                weaponsTimerL = firingIntervalL;
                weaponsTimerR = firingIntervalR;
            }
        }

        //fire left gun 
        /*if (Input.GetMouseButton(0) || inputDevice.LeftTrigger || inputDevice.LeftBumper)
        {
            //check can fire 
            if (weaponsTimerL < 0)
            {
                //if we have bullets left 
                if (bulletCount > 0)
                {
                    //guns fire 
                    guns[0].SpawnBullet();

                    if (countingBullets)
                    {
                        //lose a bullet for every gun fired 
                        bulletCount--;
                        //set bullet text 
                        bText.text = bulletCount.ToString();
                    }
                }
                //click click 
                else
                {
                    PlaySoundRandomPitch(outOfAmmoClick, 1f);
                }

                weaponsTimerL = firingIntervalL;
            }
        }

        //fire right gun 
        if (Input.GetMouseButton(1) || inputDevice.RightTrigger || inputDevice.RightBumper)
        {
            //check can fire 
            if (weaponsTimerR < 0)
            {
                //if we have bullets left 
                if (bulletCount > 0)
                {
                    //guns fire 
                    guns[1].SpawnBullet();
                    if (countingBullets)
                    {
                        //lose a bullet for every gun fired 
                        bulletCount --;
                        //set bullet text 
                        bText.text = bulletCount.ToString();
                    }
                }
                //click click 
                else
                {
                    PlaySoundRandomPitch(outOfAmmoClick, 1f);
                }

                weaponsTimerR = firingIntervalR;
            }
        }*/
    }

    
    public void LockOnToTarget(Transform deityTarget = null)
    {
        if (!useLockOnTargeting)
        {
            return;
        }

        if (lockOnTarget != deityTarget)
        {
            lockOnTarget = deityTarget;

            if (lockOnTarget != null)
            {
                targetFollow.SetXFollow(false);
                targetFollow.SetYFollow(false);
            }
            else
            {
                targetFollow.SetXFollow(true);
                targetFollow.SetYFollow(true);
            }
        }
    }
    
    #endregion

    #region Movement
    
    public void EnableControls()
    {
        controlsActive = true;
    }

    public void DisableControls()
    {
        controlsActive = false;
    }

    private Vector3 lastVelocity;
    public void FreezeMovement()
    {
        movementFrozen = true;
        lastVelocity = planeBody.velocity;
        planeBody.velocity = Vector3.zero;
        CheckAnimations();
    }

    public void ResumeMovement()
    {
        movementFrozen = false;
        planeBody.velocity = lastVelocity;
    }

    //wasd input
    float horizontal;
    float vertical;

    void MovementInputs()
    {
        //set input device
        inputDevice = InputManager.ActiveDevice;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            //grab inputs 
            horizontal = inputDevice.LeftStickX;
            vertical = inputDevice.LeftStickY;
        }
        //keyboard
        else
        {
            //grab inputs 
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        //barrel roll check - must be moving on horizontal axis 
        if (horizontal != 0)
        {
            //Barrel roll only possible with horizontal inputs
            if (Input.GetKey(KeyCode.Space) || inputDevice.Action1)
            {
                barrelRoll = true;
            }
        }
    }

    void SimpleFlight()
    {
        //todo https://shakiroslann.medium.com/unity-c-tutorial-simple-flying-control-647798a9f50e 
    }

    /// <summary>
    /// Actual application of the calculated input forces to the plane's rigidbody. 
    /// </summary>
    void ApplyForces()
    {
        //TODO if we just hold left/right, we eventually reach a critical velocity so high that it keeps pushing us offscreen :(

        HorizontalMovement();
        VerticalMovement();

        if(horizontal == 0 && vertical == 0)
        {
            SlowdownX();
        }
        //ForwardMovement();
    }

    void HorizontalMovement()
    {
        //Horizontal
        //right
        if (horizontal > 0)
        {
            //zero x vel if it is less than 0
            if (planeBody.velocity.x < 0)
            {
                SlowdownX();
            }

            //only add rightward force if we are less than x max pos and max vel
            if (transform.position.x < xMax && Mathf.Abs(planeBody.velocity.x) < maxVelocityX)
                planeBody.AddForce(horizontal * strafeSpeed, 0, 0);
            //when greater than x max and still moving right
            else if(transform.position.x > xMax && planeBody.velocity.x > 0)
                SlowdownX();
        }
        //left
        else if (horizontal < 0)
        {
            //zero x vel if it is greater than 0
            if (planeBody.velocity.x > 0)
            {
                SlowdownX();
            }

            //only add leftward force if we are greater than x min pos
            if (transform.position.x > xMin && Mathf.Abs(planeBody.velocity.x) < maxVelocityX)
                planeBody.AddForce(horizontal * strafeSpeed, 0, 0);
            //when greater kess than x min and still moving left
            else if (transform.position.x < xMin && planeBody.velocity.x < 0)
                SlowdownX();
        }

        //Exhume barrel roll -- TODO should block new barrel rolls during barrel roll? 
        if (barrelRoll)
        {
            planeBody.AddForce(new Vector3(barrelRollForce * horizontal, 0, 0), ForceMode.Impulse);
            _Animations.Animator.SetTrigger("barrelRoll");
            barrelRoll = false;
        }
    }

    /// <summary>
    /// move toward 0 vel 
    /// </summary>
    void SlowdownX()
    {
        planeBody.velocity *= xDecel;
    }

    void VerticalMovement()
    {
        //Vertical
        //up
        if (vertical > 0)
        {
            //zero y vel if it is less than 0
            if (planeBody.velocity.y < 0)
            {
                planeBody.velocity = new Vector3(planeBody.velocity.x, 0, planeBody.velocity.z);
            }

            //only add upward force if we are less than height max pos 
            if (transform.position.y < heigtMax && Mathf.Abs(planeBody.velocity.y) < maxVelocityY)
                planeBody.AddForce(0, vertical * strafeSpeed, 0);
        }
        //down
        else if (vertical < 0)
        {
            //zero y vel if it is greater than 0 OR if plane is below height min on the y
            if (planeBody.velocity.y > 0
                || transform.position.y < heightMin)
            {
                planeBody.velocity = new Vector3(planeBody.velocity.x, 0, planeBody.velocity.z);
            }

            //only add downward force if we are greater than height min pos 
            if (transform.position.y > heightMin && Mathf.Abs(planeBody.velocity.y) < maxVelocityY)
                planeBody.AddForce(0, vertical * strafeSpeed, 0);
        }
        //zero input - zero y vel
        else if (vertical == 0)
        {
            planeBody.velocity = new Vector3(planeBody.velocity.x, 0, planeBody.velocity.z);
        }
    }

    void ForwardMovement()
    {
        //forward
        if (Mathf.Abs(planeBody.velocity.z) < maxVelocityZ)
            planeBody.AddForce(0, 0, moveSpeed);
        //artificially restrict player's velocity when it exceeds max
        else
        {
            Vector3 properVel = new Vector3(planeBody.velocity.x, planeBody.velocity.y, maxVelocityZ);
            planeBody.velocity = Vector3.MoveTowards(planeBody.velocity, properVel, 50 * Time.fixedDeltaTime);
        }
    }

    //TODO should use values between -1 and 1 based on the actual Velocity of the plane, rather than the Input value to animate. 
    void CheckAnimations()
    {
        _Animations.Animator.SetFloat("Move X", planeBody.velocity.x / velocityAnimatorFactor);
        _Animations.Animator.SetFloat("Move Y", planeBody.velocity.y / velocityAnimatorFactor);
    }

    public void SetZVelMax(float amount)
    {
        maxVelocityZ = amount;
    }
    
    #endregion

    #region ZapEffect
    //zap effect
    public ParticleSystem zaps;
    IEnumerator zap;
    
    //called by lightning to zap the plane 
    public void InitiateZap()
    {
        if (zap != null)
            StopCoroutine(zap);

        zap = Zap();

        StartCoroutine(zap);

        Debug.Log("zapped!");
    }

    //fries the controls and weapons for a period of time 
    IEnumerator Zap()
    {
        controlsActive = false;

        //zero the inputs too
        horizontal = 0;
        vertical = 0;

        zaps.Play();

        PlaySoundRandomPitch(zapped, 1f);

        yield return new WaitForSeconds(2f);

        controlsActive = true;

        zaps.Stop();
    }
    #endregion
}
