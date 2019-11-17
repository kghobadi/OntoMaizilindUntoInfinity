using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThePilot : AudioHandler {

    Animator planeAnimator;
    PilotAnimation animationScript;
    public float moveSpeed;
    public float heightMin, heigtMax;
    public float xMin, xMax;

    bool input;
    //weapons 
    public Gun[] guns;
    public float weaponsTimer, firingInterval;

    public override void Awake()
    {
        base.Awake();
        planeAnimator = GetComponent<Animator>();
        animationScript = GetComponent<PilotAnimation>();
        guns = GetComponentsInChildren<Gun>();
    }
    
	//could play sounds when moving 
	void Update () {
        Movement();

        FireWeapons();

        weaponsTimer -= Time.deltaTime;
    }

    void FireWeapons()
    {
        //fire on space 
        if (Input.GetKey(KeyCode.Space))
        {
            //check can fire 
            if(weaponsTimer < 0)
            {
                for (int i = 0; i < guns.Length; i++)
                {
                    guns[i].SpawnBullet();
                }

                weaponsTimer = firingInterval;
            }
            
        }
    }

    void Movement()
    {
        //LEFT
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                new Vector3(xMin, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime); 

            animationScript.SetAnimator("left");
            input = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            input = false;
        }

        //RIGHT
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                new Vector3(xMax, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime);

            animationScript.SetAnimator("right");
            input = true;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            input = false;
        }

        //UP
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                new Vector3(transform.position.x, heigtMax, transform.position.z), moveSpeed * Time.deltaTime);

            animationScript.SetAnimator("up");
            input = true;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            input = false;
        }

        //DOWN
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                new Vector3(transform.position.x, heightMin, transform.position.z), moveSpeed * Time.deltaTime);

            animationScript.SetAnimator("down");
            input = true;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            input = false;
        }

        //IDLE
        if (input == false)
        {
            animationScript.SetAnimator("idle");
        }
    }

}
