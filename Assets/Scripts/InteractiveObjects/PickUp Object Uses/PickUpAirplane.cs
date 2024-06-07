using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAirplane : PickUpObject
{
    //private Animator airplaneAnimator;
    
    private enum AirplaneStates
    {
        IDLE,
        FLYING,
        FALLING
    }

    [Header("Airplane Settings")] 
    [SerializeField] private AirplaneStates airplaneState;
    [SerializeField] private float throwSpeed = 3f;
    [SerializeField] private float flyAccel = 1f;
    [SerializeField] private float flyLiftMin = 4f;
    [SerializeField] private float flyLiftMax = 12f;
    [SerializeField] private TrailRenderer[] airplaneTrails;
	private float flyLift;

    protected override void Start()
    {
        base.Start();
        
        //get animator
        //airplaneAnimator = GetComponent<Animator>();
        
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
		
		flyLift=Random.Range(flyLiftMin,flyLiftMax);
        
        //first drop it
        DropObject();
        
        //burst of force
        _rigidbody.AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
        
        //throw it (and enable trail renderers
        EnableFlying();
        
        TriggerInteractEvent();
    }

    private void FixedUpdate()
    {
        //Flying state
        if (airplaneState == AirplaneStates.FLYING)
        {
            //forward force over time.
            _rigidbody.AddRelativeForce(0, flyLift, flyAccel, ForceMode.Acceleration );
            
        }
    }

    void EnableFlying()
    {
		_rigidbody.angularVelocity = Vector3.zero;
        foreach (var airTrail in airplaneTrails)
        {
            airTrail.emitting = true;
        }

        //airplaneAnimator.speed = 5;

        airplaneState = AirplaneStates.FLYING;
    }
    
    void DisableFlying(bool atStart)
    {
        foreach (var airTrail in airplaneTrails)
        {
            airTrail.emitting = false;
        }
        //airplaneAnimator.speed = 1;
        
        if (!atStart)
        {
            //set falling state
            airplaneState = AirplaneStates.FALLING;
            
            //repulse airplane (bounce)
            //_rigidbody.AddRelativeForce(0, 0, -flyAccel * throwMultiplier);
        
            //zero angular vel
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    void SetIdle()
    {
        //zero velocity 
        //_rigidbody.velocity = Vector3.zero;
        //_rigidbody.angularVelocity = Vector3.zero;
        //idle
        airplaneState = AirplaneStates.IDLE;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //only disable flying if the collision obj is not equal to
        if (!collision.gameObject.CompareTag("Player"))
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
