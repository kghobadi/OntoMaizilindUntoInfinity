using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAirplane : PickUpObject
{
    private Animator airplaneAnimator;
    [Header("Airplane Settings")] 
    public TrailRenderer[] airplaneTrails;
    public bool flying;
    public float throwMultiplier = 5f;
    public float flySpeed;
    public float maxVelocityZ;

    protected override void Start()
    {
        base.Start();
        
        //get animator
        airplaneAnimator = GetComponent<Animator>();
        
        //make sure not flying
        DisableFlying();
    }

    /// <summary>
    /// Throws the airplane.
    /// </summary>
    public override void UseObject()
    {
        base.UseObject();
        
        //first drop it
        DropObject();
        
        //burst of force
        _rigidbody.AddRelativeForce(0, 0, flySpeed  * throwMultiplier);
        
        //throw it (and enable trail renderers
        EnableFlying();
    }

    private void FixedUpdate()
    {
        if (flying)
        {
            //forward force over time.
            if (Mathf.Abs(_rigidbody.velocity.z) < maxVelocityZ)
            {
                _rigidbody.AddRelativeForce(0, 0, flySpeed );
            }
            //artificially restrict player's velocity when it exceeds max
            else
            {
                //velocity limit
                Vector3 properVel = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, maxVelocityZ);
                _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, properVel, 50 * Time.deltaTime);
            }
        }
    }

    void EnableFlying()
    {
        foreach (var airTrail in airplaneTrails)
        {
            airTrail.emitting = true;
        }

        airplaneAnimator.speed = 5;
        flying = true;
    }
    
    void DisableFlying()
    {
        foreach (var airTrail in airplaneTrails)
        {
            airTrail.emitting = false;
        }
        airplaneAnimator.speed = 1;
        flying = false;

        //repulse airplane (bounce)
        _rigidbody.AddRelativeForce(0, 0, -flySpeed * throwMultiplier);
        
        //zero angular vel
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //only disable flying if the collision obj is not equal to
        if (collision.gameObject.CompareTag("Player") == false)
        {
            //bumped into something while flying
            if (flying)
            {
                DisableFlying();
            }
            else
            {
                //zero velocity 
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }
       
    }
}
