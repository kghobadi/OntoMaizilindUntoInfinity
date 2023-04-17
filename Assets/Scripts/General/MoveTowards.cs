using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour 
{
    public float moveSpeed;
    [HideInInspector] public float origSpeed;

    public bool moving;
    public Vector3 destination;
    private Transform destTransform;
    public float necessaryDist = 1f;

    public bool increaseSpeedOverTime;
    public float speedIncrease;

    void Start()
    {
        origSpeed = moveSpeed;
    }

    void FixedUpdate() 
    {
        if (moving)
        {
            //constantly update dest if we have a transform
            if (destTransform != null)
            {
                destination = destTransform.position;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);

            //add to speed over time. 
            if (increaseSpeedOverTime)
            {
                moveSpeed += speedIncrease * Time.deltaTime;
            }

            float currentDist = Vector3.Distance(transform.position, destination);
            if(currentDist < necessaryDist)
            {
                moving = false;
                moveSpeed = origSpeed;
                destTransform = null;
            }
        }
	}

    public void MoveTo(Vector3 dest, float speed)
    {
        destination = dest;
        moveSpeed = speed;
        moving = true;
    }
    
    public void MoveTo(Transform dest, float speed)
    {
        destination = dest.position;
        destTransform = dest;
        moveSpeed = speed;
        moving = true;
    }
}
