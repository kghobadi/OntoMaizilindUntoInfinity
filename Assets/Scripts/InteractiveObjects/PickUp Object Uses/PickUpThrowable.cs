using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpThrowable : PickUpObject
{
	public float throwLift = 3f;
	public float throwSpeed = 5f;
	public float angularVelX = 7f;
	public float angularVelRand = 2f;

	protected override void SetActive()
	{
		if(_rigidbody.velocity.magnitude < 1f)
			base.SetActive();
	}

	public override void UseObject()
    {
        base.UseObject();
        
        //first drop it
        DropObject();
        
        //burst of force
        _rigidbody.AddRelativeForce(0, throwLift, throwSpeed, ForceMode.VelocityChange);
        _rigidbody.AddRelativeTorque( new Vector3(angularVelX,0f,0f) + (Random.onUnitSphere*angularVelRand) , ForceMode.VelocityChange);
        
        TriggerInteractEvent();
    }
}
