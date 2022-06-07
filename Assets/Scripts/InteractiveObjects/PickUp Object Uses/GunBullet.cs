using System;
using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;

/// <summary>
/// Controls bullet fired from the Gun.
/// </summary>
public class GunBullet : MonoBehaviour
{
    private Rigidbody bulletBody;
    private BoxCollider bulletCollider;
    public TrailRenderer bulletTrail;
    public bool shooting;
    public MovementPath killPerson;
    public ParticleSystem bloodSpatter;
    public float fireForce = 500f;
    private float residualFireForce;
    public float maxVelocityZ = 100f;
    
    void Start()
    {
        bulletBody = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<BoxCollider>();
        DisableMovement();
    }

    private void FixedUpdate()
    {
        if (shooting)
        {
            //collider must be enabled
            bulletCollider.enabled = true;
            
            //forward force over time.
            if (Mathf.Abs(bulletBody.velocity.z) < maxVelocityZ)
            {
                bulletBody.AddRelativeForce(0, 0, residualFireForce );
            }
            //artificially restrict player's velocity when it exceeds max
            else
            {
                //velocity limit
                Vector3 properVel = new Vector3(bulletBody.velocity.x, bulletBody.velocity.y, maxVelocityZ);
                bulletBody.velocity = Vector3.MoveTowards(bulletBody.velocity, properVel, 50 * Time.deltaTime);
            }
        }
    }

    public void FireBullet()
    {
        transform.SetParent(null);
        bulletTrail.emitting = true;
        bulletBody.isKinematic = false;
        bulletBody.AddRelativeForce(0,0,fireForce);
        residualFireForce = fireForce / 2;

        shooting = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //hit something..
        if (collision.gameObject.CompareTag("Human") && shooting)
        {
            HitPerson(collision.gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //hit something..
        if (other.gameObject.CompareTag("Human") && shooting)
        {
            HitPerson(other.gameObject);
        }
    }

    void HitPerson(GameObject person)
    {
        Movement personAI = person.GetComponent<Movement>();
        
        personAI.ResetMovement(killPerson);
        
        bloodSpatter.Play();
        
        DisableMovement();
        
        Destroy(gameObject);
    }

    public void DisableMovement()
    {
        //disable movement
        bulletBody.isKinematic = true;
        bulletTrail.emitting = false;
        bulletBody.velocity = Vector3.zero;

        shooting = false;
    }
}
