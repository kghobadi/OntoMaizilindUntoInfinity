using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour 
{
    public float moveSpeed;
    [HideInInspector] public float origSpeed;

    public bool moving;
    public Vector3 destination;
    public float necessaryDist = 1f;

    public bool increaseSpeedOverTime;
    public float speedIncrease;

    void Start()
    {
        origSpeed = moveSpeed;
    }

    void Update () 
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);

            //add to speed over time. 
            if (increaseSpeedOverTime)
            {
                moveSpeed += speedIncrease * Time.deltaTime;
            }
            
            if(Vector3.Distance(transform.position, destination) < necessaryDist)
            {
                moving = false;
                moveSpeed = origSpeed;
            }
        }
	}

    public void MoveTo(Vector3 dest, float speed)
    {
        destination = dest;
        moveSpeed = speed;
        moving = true;
    }
}
