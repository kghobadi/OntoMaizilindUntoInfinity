using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    public bool lockRotation;
    public Vector3 rotationToLock;
    
    public bool lookAt;
    public float lookAdjust = 25f;
    public GameObject objToLookAt;

    public void SetLock()
    {
        lockRotation = true;
    }
    
    public void SetUnlock()
    {
        lockRotation = false;
    }

    public void SetLookAdjust(float adjust)
    {
        lookAdjust = adjust;
    }
    
    void Update()
    {
        if (lockRotation)
        {
            if (lookAt)
            {
                Vector3 lookSpot = new Vector3(objToLookAt.transform.position.x, transform.position.y, objToLookAt.transform.position.z);
                transform.LookAt(lookSpot);
                transform.Rotate(0f, lookAdjust,0f);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(rotationToLock);
            }
        }
    }
}
