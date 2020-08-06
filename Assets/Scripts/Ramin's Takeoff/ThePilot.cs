using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using InControl;

public class ThePilot : AudioHandler {
    
    PilotAnimation _Animations;
    PilotView pView;
    MeshRenderer planeRender;
    AdvanceScene advance;

    [Header("Movement & Inputs")]
    public float moveSpeed;
    public float heightMin, heigtMax;
    public float xMin, xMax;
    public bool controlsActive = true;
    public ParticleSystem zaps;
    IEnumerator zap;
    bool input;
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

    public override void Awake()
    {
        base.Awake();
        planeRender = GetComponent<MeshRenderer>();
        _Animations = GetComponent<PilotAnimation>();
        advance = FindObjectOfType<AdvanceScene>();
        pView = FindObjectOfType<PilotView>();
        guns = GetComponentsInChildren<Gun>();
        bText.text = bulletCount.ToString();
        SwitchViews(false);
    }
    
	//could play sounds when moving 
	void Update () {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        Movement();

        FireWeapons();

        weaponsTimerL -= Time.deltaTime;
        weaponsTimerR -= Time.deltaTime;

        //switch views
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) 
            ||  inputDevice.DPadLeft.WasPressed || inputDevice.DPadRight.WasPressed
            || inputDevice.DPadUp.WasPressed || inputDevice.DPadDown.WasPressed)
        {
            ToggleViews();
        }

        if(music.isPlaying  == false)
        {
            advance.LoadNextScene();
        }
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
            pView.isActive = true;
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
            pView.isActive = false;
            zoomedIn = false;
        }
    }

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

    void FireWeapons()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //fire on space 
        if ((Input.GetKey(KeyCode.Space) || inputDevice.Action1) && controlsActive)
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
                    //lose a bullet for every gun fired 
                    bulletCount -= guns.Length;
                    //set bullet text 
                    bText.text = bulletCount.ToString();
                }
                //click click 
                else
                {
                    PlaySoundRandomPitch(outOfAmmoClick, 1f);

                    advance.LoadNextScene();
                }

                weaponsTimerL = firingIntervalL;
                weaponsTimerR = firingIntervalR;
            }
        }

        //fire left gun 
        if ((Input.GetMouseButton(0) || inputDevice.LeftTrigger || inputDevice.LeftBumper) && controlsActive)
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
        if ((Input.GetMouseButton(1) || inputDevice.RightTrigger || inputDevice.RightBumper) && controlsActive)
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

    void Movement()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        float horizontal;
        float vertical;

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

        //set mouse vars
        float mouseX = 0;
        float mouseY = 0;
        //if we are not in first person;
        if (!zoomedIn)
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }

        //controls must be active
        if (controlsActive)
        {
            //LEFT
            if (horizontal < 0 || mouseX < 0)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(xMin, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime);
            }
            //RIGHT
            if (horizontal > 0 || mouseX > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(xMax, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime);
            }
            //UP
            if (vertical > 0 || mouseY > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(transform.position.x, heigtMax, transform.position.z), moveSpeed * Time.deltaTime);
            }
            //DOWN
            if (vertical < 0 || mouseY < 0)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(transform.position.x, heightMin, transform.position.z), moveSpeed * Time.deltaTime);
            }
        }
        
        //always MOVE FORWARD 
        transform.position = Vector3.MoveTowards(transform.position,
               new Vector3(transform.position.x, transform.position.y, transform.position.z + 100f), moveSpeed * Time.deltaTime);

        //no input -- IDLE
        if (vertical ==0 && horizontal == 0 && mouseX == 0 && mouseY == 0)
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
            //set animator floats for blend  MOUSE
            if (mouseX != 0 || mouseY != 0)
            {
                _Animations.SetAnimator("moving");
                _Animations.characterAnimator.SetFloat("Move X", mouseX);
                _Animations.characterAnimator.SetFloat("Move Y", mouseY);
            }
        }
       
    }

}
