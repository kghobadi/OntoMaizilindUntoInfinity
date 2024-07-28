using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPosition : MonoBehaviour
{
    private Vector3 origPosition;
    private Vector3 origLocalPosition;
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;
    [SerializeField] private bool lockXLocal;
    [SerializeField] private bool lockYLocal;
    [SerializeField] private bool lockZLocal;
    
    void Start()
    {
        origPosition = transform.position;
        origLocalPosition = transform.localPosition;
    }
    
    void Update()
    {
        //World
        Vector3 newCurrentPos = transform.position;
        if (lockX)
        {
            newCurrentPos.x = origPosition.x;
        }   
        if (lockY)
        {
            newCurrentPos.y = origPosition.y;
        }   
        if (lockZ)
        {
            newCurrentPos.z = origPosition.z;
        }   
        
        //Local
        Vector3 newLocalPos = transform.localPosition;
        if (lockXLocal)
        {
            newLocalPos.x = origLocalPosition.x;
        }   
        if (lockYLocal)
        {
            newLocalPos.y = origLocalPosition.y;
        }   
        if (lockZLocal)
        {
            newLocalPos.z = origLocalPosition.z;
        }   
        
        transform.position = newCurrentPos;
        transform.localPosition = newLocalPos;
    }
}
