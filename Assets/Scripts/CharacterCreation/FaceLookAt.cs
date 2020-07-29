//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//​
//public class FaceLookAt : MonoBehaviour
//{
//    Transform realP, thisBody;
//    SpriteRenderer s;
//    public bool mantenerseParado;
//    public bool changeSprites;
//    public bool flipSide;
//​
//    public Sprite front, right, left, back;

//    void Start()
//    {
//        realP = Camera.main.transform;
//        thisBody = transform.parent.parent;​
//        s = GetComponent<SpriteRenderer>();​
//    }

//    void Update()
//    {​
//        transform.LookAt(realP);

//        if (mantenerseParado)
//            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
//​
//        if (changeSprites)
//        {
//            //find the angle from player to this
//            float angle = Vector3.SignedAngle(thisBody.forward, realP.position - thisBody.position, Vector3.up);

//            s.flipX = false;
//            if (angle > -45 && angle < 45)
//                s.sprite = front;
//            else if (angle > 45 && angle < 135)
//                s.sprite = left;
//            else if (angle > -135 && angle < -45)
//            {
//                s.sprite = right;
//                s.flipX = true;
//            }
//            else if (angle > 135)
//                s.sprite = back;
//            else if (angle < -135)
//                s.sprite = back;​
//        }​
//    }
//}