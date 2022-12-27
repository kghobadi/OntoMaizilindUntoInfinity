using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAirplane : PickUpObject
{
    private Animator airplaneAnimator;
    
    private enum AirplaneStates
    {
        IDLE,
        FLYING,
        FALLING
    }

    [Header("Airplane Settings")] 
    [SerializeField] private AirplaneStates airplaneState;
    [SerializeField] private float throwMultiplier = 5f;
    [SerializeField] private float flySpeed;
    [SerializeField] private float maxVelocityZ;
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float maxVelocityY = 8f;
    [SerializeField] private TrailRenderer[] airplaneTrails;

    protected override void Start()
    {
        base.Start();
        
        //get animator
        airplaneAnimator = GetComponent<Animator>();
        
        //make sure not flying
        DisableFlying(true);
        
        //start in Idle 
        SetIdle();
    }

    public override void HoldItem()
    {
        //ensure idle while holding 
        SetIdle();
        
        base.HoldItem();
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
        //Flying state
        if (airplaneState == AirplaneStates.FLYING)
        {
            //forward force over time.
            if (Mathf.Abs(_rigidbody.velocity.z) < maxVelocityZ)
            {
                _rigidbody.AddRelativeForce(0, 0, flySpeed );
            }
            //artificially restrict plane's velocity when it exceeds max
            else
            {
                //velocity limit
                Vector3 properVel = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, maxVelocityZ);
                _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, properVel, 50 * Time.deltaTime);
            }
        }
        //Falling state
        else if (airplaneState == AirplaneStates.FALLING)
        {
            //downward force over time.
            if (Mathf.Abs(_rigidbody.velocity.y) < maxVelocityY)
            {
                _rigidbody.AddForce(0, -fallSpeed,  0f);
            }
            //artificially restrict plane's velocity when it exceeds max
            else
            {
                //velocity limit
                Vector3 properVel = new Vector3(_rigidbody.velocity.x, maxVelocityY, _rigidbody.velocity.z);
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

        airplaneState = AirplaneStates.FLYING;
    }
    
    void DisableFlying(bool atStart)
    {
        foreach (var airTrail in airplaneTrails)
        {
            airTrail.emitting = false;
        }
        airplaneAnimator.speed = 1;
        
        if (!atStart)
        {
            //set falling state
            airplaneState = AirplaneStates.FALLING;
            
            //repulse airplane (bounce)
            _rigidbody.AddRelativeForce(0, 0, -flySpeed * throwMultiplier);
        
            //zero angular vel
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    void SetIdle()
    {
        //zero velocity 
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        //idle
        airplaneState = AirplaneStates.IDLE;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //only disable flying if the collision obj is not equal to
        if (collision.gameObject.CompareTag("Player") == false)
        {
            //bumped into something while flying
            if (airplaneState == AirplaneStates.FLYING)
            {
                DisableFlying(false);
            }
            //hit something while falling
            else if(airplaneState == AirplaneStates.FALLING)
            {
                SetIdle();
            }
        }
    }
}
