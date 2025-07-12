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
    public float maxVelocityXY = 100f;
    public float heightMin, heigtMax;
    public float xMin, xMax;
    //todo add velocity max for x/y and cap it at 666
    public float maxVelocityX = 666f;
    public float maxVelocityY = 666f;
    [SerializeField]
    private float xDecel = 0.8f;
    [SerializeField]
    private float yDecel = 0.8f;

    [SerializeField] private Animator steeringWheel;
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
    public CanvasFader weaponsControls;
    [Tooltip("Controls UI which appears at start of sequence")] 
    public CanvasFader rollControls;
    public CanvasFader moveControls;

    [Tooltip("Shows plane interior to player while in Third person")]
    public CanvasGroup fpsView;

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
    public AudioClip barrelRollSfx;

    [SerializeField]
    private MonologueManager raminMonos;

    public MonologueManager PilotMonos => raminMonos;
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

            //Debug.Log("X velocity is " + planeBody.velocity.x);
            //Debug.Log("Y velocity is " + planeBody.velocity.y);
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
            //Fade out visual 
            if (fpsView)
            {
                LeanTween.alphaCanvas(fpsView, 0f, 1f);
            }
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
            //Fade in visual 
            if (fpsView)
            {
                LeanTween.alphaCanvas(fpsView, 1f, 1f);
            }
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
                    if (weaponsControls.IsShowing)
                    {
                        weaponsControls.FadeOut();
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

    public void ResumeMovement(bool useLastVel = false)
    {
        movementFrozen = false;
        
        if(useLastVel)
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
            //Barrel roll only possible with horizontal inputs AND NOT ZAPPED!
            if ((Input.GetKeyDown(KeyCode.Space) || inputDevice.Action1.WasPressed) 
                && !isZapped)
            {
                barrelRoll = true;
            }
        }

        //Fade out move controls. 
        if (moveControls.IsShowing && horizontal != 0 || vertical != 0)
        {
            moveControls.FadeOut();
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

        //Apply correct slow downs for zeroed movement input. 
        if(horizontal == 0 && vertical == 0)
        {
            Slowdown();
        }
        else if (horizontal == 0)
        {
            SlowdownX();
        }
        else if (vertical == 0)
        {
            SlowdownY();
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
                HorizontalMove();
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
                HorizontalMove();
            //when less than x min and still moving left
            else if (transform.position.x < xMin && planeBody.velocity.x < 0)
                SlowdownX();
        }

        //Exhume barrel roll 
        if (barrelRoll && !_Animations.IsInBarrelRoll)
        {
            DoABarrelRoll();
        }
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
                SlowdownY();
            }

            //only add upward force if we are less than height max pos 
            if (transform.position.y < heigtMax && Mathf.Abs(planeBody.velocity.y) < maxVelocityY)
                VerticalMove();
            //when greater than y max and still moving up
            else if(transform.position.y > heigtMax && planeBody.velocity.y > 0)
                SlowdownY();
        }
        //down
        else if (vertical < 0)
        {
            //zero y vel if it is greater than 0 OR if plane is below height min on the y
            if (planeBody.velocity.y > 0
                || transform.position.y < heightMin)
            {
                SlowdownY();
            }

            //only add downward force if we are greater than height min pos 
            if (transform.position.y > heightMin && Mathf.Abs(planeBody.velocity.y) < maxVelocityY)
                VerticalMove();
            //when less than y min and still moving down
            else if (transform.position.y < heightMin && planeBody.velocity.y < 0)
                SlowdownY();
        }
    }

    /// <summary>
    /// Add our horizontal movement force. 
    /// </summary>
    void HorizontalMove()
    {
        planeBody.AddForce(horizontal * strafeSpeed, 0, 0);
    }

    /// <summary>
    /// Add our vertical movement force. 
    /// </summary>
    void VerticalMove()
    {
        planeBody.AddForce(0, vertical * strafeSpeed, 0);
    }

    /// <summary>
    /// What you think it does. 
    /// </summary>
    void DoABarrelRoll()
    {
        planeBody.AddForce(new Vector3(barrelRollForce * horizontal, 0, 0), ForceMode.Impulse);
        _Animations.Animator.SetTrigger("barrelRoll");
        _Animations.IsInBarrelRoll = true;
        barrelRoll = false;
        
        //fade out all weapons controls UIs 
        if (rollControls.IsShowing)
        {
            rollControls.FadeOut();
        }
        
        //cool sound effect for this!
        PlaySoundRandomPitch(barrelRollSfx, 1f);
    }

    //TODO should use values between -1 and 1 based on the actual Velocity of the plane, rather than the Input value to animate. 
    void CheckAnimations()
    {
        // _Animations.Animator.SetFloat("Move X", horizontal);
        // _Animations.Animator.SetFloat("Move Y", vertical);
        _Animations.Animator.SetFloat("Move X", planeBody.velocity.x / velocityAnimatorFactor);
        _Animations.Animator.SetFloat("Move Y", planeBody.velocity.y / velocityAnimatorFactor);
        steeringWheel.SetFloat("Move X", horizontal);
        steeringWheel.SetFloat("Move Y", vertical);
    }
    
    /// <summary>
    /// move x toward 0 vel 
    /// </summary>
    void SlowdownX()
    {
        planeBody.velocity = new Vector3(planeBody.velocity.x * xDecel, planeBody.velocity.y, planeBody.velocity.z);
    }
    
    /// <summary>
    /// move y toward 0 vel 
    /// </summary>
    void SlowdownY()
    {
        planeBody.velocity = new Vector3(planeBody.velocity.x, planeBody.velocity.y * yDecel, planeBody.velocity.z);
    }
    
    /// <summary>
    /// move toward 0 vel 
    /// </summary>
    void Slowdown()
    {
        planeBody.velocity *= xDecel;
    }
    /// <summary>
    /// Set x to 0 vel 
    /// </summary>
    void ZeroX()
    {
        planeBody.velocity = new Vector3(0f, planeBody.velocity.y, planeBody.velocity.z);
    }

    /// <summary>
    /// Set y to 0 vel 
    /// </sumary>
    void ZeroY()
    {
        planeBody.velocity = new Vector3(planeBody.velocity.x, 0f, planeBody.velocity.z);
    }

    /// <summary>
    /// Zero the velocity. 
    /// </summary>
    void ZeroVelocity()
    {
        planeBody.velocity = Vector3.zero;
    }
    
    #endregion

    #region ZapEffect
    //zap effect
    public ParticleSystem zaps;
    private bool isZapped;
    IEnumerator zap;
    
    //called by lightning to zap the plane 
    public void InitiateZap()
    {
        //Early return when already zapped OR in barrel roll OR the controls are inactive. 
        if (isZapped || _Animations.IsInBarrelRoll || !controlsActive)
        {
            return;
        }
        
        if (zap != null)
            StopCoroutine(zap);

        zap = Zap();

        StartCoroutine(zap);

        Debug.Log("zapped!");
    }

    //freeze the movement and play zap effect
    IEnumerator Zap()
    {
        isZapped = true;
        FreezeMovement();
        
        zaps.Play();
        PlaySoundRandomPitch(zapped, 1f);

        yield return new WaitForSeconds(2f);

        zaps.Stop();
        ResumeMovement();
        isZapped = false;
    }
    #endregion
}
