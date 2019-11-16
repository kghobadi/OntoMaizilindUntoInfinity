using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThePilot : AudioHandler {

    Animator planeAnimator;
    PilotAnimation animationScript;
    public float moveSpeed;

    bool input;

    public override void Awake()
    {
        base.Awake();
        planeAnimator = GetComponent<Animator>();
        animationScript = GetComponent<PilotAnimation>();
    }
    
	//could play sounds when moving 
	void Update () {
        //LEFT
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(-5, 0, 0), moveSpeed * Time.deltaTime);

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
            transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(5, 0, 0), moveSpeed * Time.deltaTime);

            animationScript.SetAnimator("right");
            input = true;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            input = false;
        }
        //IDLE
        if(input == false)
        {
            animationScript.SetAnimator("idle");
        }
    }

}
