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
    public float maxVelocityZ = 125f;
    public float maxVelocityXY = 100f;
    public float heightMin, heigtMax;
    public float xMin, xMax;
    public float mouseFactor = 3f;
    public bool controlsActive = true;
    public bool countingBullets;
    InputDevice inputDevice;

    //weapons 
    [Header("Weapons")]
    public Gun[] guns;
    [Tooltip("Time between bullets firing left")]
    public float weaponsTimerL, firingIntervalL = 0.05f;
    [Tooltip("Time between bullets firing right")]
    public float weaponsTimerR, firingIntervalR = 0.05f;
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
        planeRender = GetComponent<MeshRenderer>();
        _Animations = GetComponent<PilotAnimation>();
        advance = FindObjectOfType<AdvanceScene>();
        planeBody = GetComponent<Rigidbody>();
        guns = GetComponentsInChildren<Gun>();
        bText.text = bulletCount.ToString();
        SwitchViews(false);
    }
    
	//could play sounds when moving 
	void Update ()
    {
        if (controlsActive)
        {
            MovementInputs();

            FireWeapons();

            //switch views
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)
                || inputDevice.DPadLeft.WasPressed || inputDevice.DPadRight.WasPressed
                || inputDevice.DPadUp.WasPressed || inputDevice.DPadDown.WasPressed)
            {
                ToggleViews();
            }
        }

        weaponsTimerL -= Time.deltaTime;
        weaponsTimerR -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        ApplyForces();

        CheckAnimations();
    }

    public void EnableControls()
    {
        controlsActive = true;
    }

    public void DisableControls()
    {
        controlsActive = false;
    }

    public void ToggleViews()
    {
        zoomedIn = !zoomedIn;
        SwitchViews(zoomedIn);
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

    void SwitchViews(bool fpORzoom)
    {
        //first person
        if (fpORzoom)
        {
            fpCam.SetActive(true);
            cockpit.SetActive(true);
            zoCam.SetActive(false);
            planeRender.enabled = false;
            //set animator 
            //_Animations.SetAnimator("idle");
            //_Animations.Animator.enabled = false;
            zoomedIn = true;
        }
        //zoomed out 
        else
        {
            fpCam.SetActive(false);
            cockpit.SetActive(false);
            zoCam.SetActive(true);
            planeRender.enabled = true;
            //_Animations.Animator.enabled = true;
            zoomedIn = false;
        }
    }

    #region Weapons

    void FireWeapons()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //fire on space 
        if ((Input.GetKey(KeyCode.Space) || inputDevice.Action1))
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
                }
                //click click 
                else
                {
                    PlaySoundRandomPitch(outOfAmmoClick, 1f);

                    //this will need to be replaced by transition 
                    triggerApocalypse.SetTrigger();
                }

                weaponsTimerL = firingIntervalL;
                weaponsTimerR = firingIntervalR;
            }
        }

        //fire left gun 
        if ((Input.GetMouseButton(0) || inputDevice.LeftTrigger || inputDevice.LeftBumper))
        {
            //check can fire 
            if (weaponsTimerL < 0)
            {
                //if we have bullets left 
                if (bulletCount > 0)
                {
                    //guns fire 
                    guns[0].SpawnBullet();
                    //lose a bullet for every gun fired 
                    bulletCount--;
                    //set bullet text 
                    bText.text = bulletCount.ToString();
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
        if ((Input.GetMouseButton(1) || inputDevice.RightTrigger || inputDevice.RightBumper))
        {
            //check can fire 
            if (weaponsTimerR < 0)
            {
                //if we have bullets left 
                if (bulletCount > 0)
                {
                    //guns fire 
                    guns[1].SpawnBullet();
                    //lose a bullet for every gun fired 
                    bulletCount --;
                    //set bullet text 
                    bText.text = bulletCount.ToString();
                }
                //click click 
                else
                {
                    PlaySoundRandomPitch(outOfAmmoClick, 1f);
                }

                weaponsTimerR = firingIntervalR;
            }
        }
    }
    #endregion

    #region Movement

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
            horizontal = inputDevice.LeftStickX;
            vertical = inputDevice.LeftStickY;
        }
        //keyboard
        else
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }
    }

    void ApplyForces()
    {
        //horizontal
        if(planeBody.velocity.x < maxVelocityXY )
            planeBody.AddForce(horizontal * strafeSpeed, 0, 0);

        //zero vel
        if(horizontal == 0)
        {
            planeBody.velocity = new Vector3(0, planeBody.velocity.y, planeBody.velocity.z);
        }

        //vertical
        if (planeBody.velocity.y < maxVelocityXY)
            planeBody.AddForce(0, vertical * strafeSpeed, 0);

        //zero vel
        if (vertical == 0)
        {
            planeBody.velocity = new Vector3(planeBody.velocity.x, 0, planeBody.velocity.z);
        }

        //forward
        if (planeBody.velocity.z < maxVelocityZ)
            planeBody.AddForce(0, 0, moveSpeed);
    }

    void CheckAnimations()
    {
        //no input -- IDLE
        if (vertical == 0 && horizontal == 0)
        {
            _Animations.SetAnimator("idle");
        }

        //must have controls active to animate in a direction
        if (controlsActive)
        {
            //set animator floats for blend WASD
            if (vertical != 0 || horizontal != 0)
            {
                _Animations.SetAnimator("moving");
                _Animations.characterAnimator.SetFloat("Move X", horizontal);
                _Animations.characterAnimator.SetFloat("Move Y", vertical);
            }

        }
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

        zaps.Play();

        PlaySoundRandomPitch(zapped, 1f);

        yield return new WaitForSeconds(2f);

        controlsActive = true;

        zaps.Stop();
    }
    #endregion
}
