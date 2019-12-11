using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThePilot : AudioHandler {

    Animator planeAnimator;
    PilotAnimation animationScript;
    MeshRenderer planeRender;
    public float moveSpeed;
    public float heightMin, heigtMax;
    public float xMin, xMax;
    public AudioClip outOfAmmoClick;
    bool input;
    //weapons 
    public Gun[] guns;
    public float weaponsTimerL, firingIntervalL;
    public float weaponsTimerR, firingIntervalR;

    public bool zoomedIn;
    public GameObject fpCam,cockpit, zoCam;

    public int bulletCount = 1800;
    public TMP_Text bText;

    public override void Awake()
    {
        base.Awake();
        planeRender = GetComponent<MeshRenderer>();
        planeAnimator = GetComponent<Animator>();
        animationScript = GetComponent<PilotAnimation>();
        guns = GetComponentsInChildren<Gun>();
        bText.text = bulletCount.ToString();
        SwitchViews(zoomedIn);
    }
    
	//could play sounds when moving 
	void Update () {
        Movement();

        FireWeapons();

        weaponsTimerL -= Time.deltaTime;
        weaponsTimerR -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            ToggleViews();
        }
    }

    void ToggleViews()
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
        }
        //zoomed out 
        else
        {
            fpCam.SetActive(false);
            cockpit.SetActive(false);
            zoCam.SetActive(true);
            planeRender.enabled = true;
        }
    }

    void FireWeapons()
    {
        //fire on space 
        if (Input.GetKey(KeyCode.Space))
        {
            //check can fire 
            if(weaponsTimerL < 0 || weaponsTimerR < 0)
            {
                //if we have bullets left 
                if(bulletCount > 0)
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
                }

                weaponsTimerL = firingIntervalL;
                weaponsTimerR = firingIntervalR;
            }
        }

        //fire left gun 
        if (Input.GetMouseButton(0))
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
        if (Input.GetMouseButton(1))
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
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

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
        if ( vertical < 0 || mouseY < 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                new Vector3(transform.position.x, heightMin, transform.position.z), moveSpeed * Time.deltaTime);
        }

        //no input -- IDLE
        if(vertical ==0 && horizontal == 0 && mouseX == 0 && mouseY == 0)
        {
            animationScript.SetAnimator("idle");
        }
        //set animator floats for blend WASD
        else if (vertical != 0 || horizontal != 0)
        {
            animationScript.SetAnimator("moving");
            animationScript.characterAnimator.SetFloat("Move X", horizontal);
            animationScript.characterAnimator.SetFloat("Move Y", vertical);
        }
        //set animator floats for blend  MOUSE
        else if (mouseX != 0 || mouseY != 0)
        {
            animationScript.SetAnimator("moving");
            animationScript.characterAnimator.SetFloat("Move X", mouseX);
            animationScript.characterAnimator.SetFloat("Move Y", mouseY);
        }
    }

}
