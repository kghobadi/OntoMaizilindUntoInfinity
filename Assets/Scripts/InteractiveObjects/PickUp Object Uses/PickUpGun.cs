using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUpGun : PickUpObject
{
    [Header("Gun Settings")] 
    public GunBullet bullet;
    public LayerMask shootableMask;

    public Animator personAnimator;
    public ParticleSystem fireGun;
    public Animator playerAnimator;

    public UnityEvent fire;
    public bool hasFired;
    public Transform target;

    public AudioClip gunShot;
    
    public override void HoldItem()
    {
        base.HoldItem();
        
        //get animator
        personAnimator = fpsHolder.personAnimator;
    }

    /// <summary>
    /// Fires the gun. 
    /// </summary>
    public override void UseObject()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, shootableMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            //only fire the gun if we are aiming at a human. 
            if (hit.transform.gameObject.CompareTag("Human") && !hasFired)
            {
                base.UseObject();
                
                //fire gun effect
                fireGun.Play();
        
                //trigger anim in person
                personAnimator.SetTrigger("FireGun");
                
                //fire the gun.
                bullet.transform.LookAt(target.position);
                bullet.FireBullet();
                
                //sound
                PlaySound(gunShot, 1f);
                
                //fired
                fire.Invoke();
                hasFired = true;
            }
        }
    }

    //makes ramin fire at himself 
    public void WaitFireGunAtTarget(float wait)
    {
        transform.LookAt(target);

        StartCoroutine(WaitToFire(wait));
    }

    IEnumerator WaitToFire(float wait)
    {
        yield return new WaitForSeconds(wait);
        
        UseObject();
    }

}
