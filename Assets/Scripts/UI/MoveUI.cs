using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUI : MonoBehaviour {
    RectTransform rectTrans;
    public float moveSpeed;
    public float origSpeed;

    public bool moving;
    public Vector3 destination;
    public float necessaryDist = 0.1f;

    void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    void Start()
    {
        origSpeed = moveSpeed;
    }

    void Update()
    {
        if (moving)
        {
            rectTrans.anchoredPosition = Vector3.Lerp(rectTrans.anchoredPosition, destination, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(rectTrans.anchoredPosition, destination) < necessaryDist)
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
