using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script to ensure AIs maintain original local position after pushing each other around.
/// </summary>
public class MaintainOriginalLocal : MonoBehaviour
{
    public bool returnToOriginalPosition;
    private Vector3 localOrig;
    private Rigidbody _rigidbody;
    [SerializeField] private float returnSpeed = 0.1f;
    private bool returning;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        localOrig = transform.localPosition;
        if (returnToOriginalPosition)
        {
            InvokeRepeating("ReturnToOrigPos", 0.5f, 0.5f);
        }
    }

    private void FixedUpdate()
    {
        if (returning)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, localOrig, returnSpeed);
            if (_rigidbody)
            {
                _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, Vector3.zero, returnSpeed);
                
            }
            
            //stop when close enough to orig pos
            float dist = Vector3.Distance(transform.localPosition, localOrig);
            if (dist < 0.35f)
            {
                returning = false;
            }
        }
    }

    void ReturnToOrigPos()
    {
        float dist = Vector3.Distance(transform.localPosition, localOrig);
        if (dist > 3)
        {
            returning = true;
        }
    }
}
