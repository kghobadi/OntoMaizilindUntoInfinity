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

    //Bullet Fire ending
    public AudioClip gunShot;
    public NPC.Movement raminAi;
    public NPC.MovementPath killPerson;
    [SerializeField] private NPC.Animations raminAnim;
    private string deathTrigger = "die";
    public ParticleSystem bloodSpatter;
    
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
        //only fire the gun once.
        if (!hasFired)
        {
            base.UseObject();
                
            //fire gun effect
            fireGun.Play();
        
            //trigger anim in person
            personAnimator.SetTrigger("FireGun");
                
            //fire the gun.
            //bullet.transform.LookAt(target.position);
            //bullet.FireBullet();
        
            //ramin AI behavior
            //raminAi.ResetMovement(killPerson);
            raminAnim.Animator.SetTrigger(deathTrigger);
            //blood particles
            if(bloodSpatter)
                bloodSpatter.Play();
                
            //sound
            PlaySound(gunShot, 1f);
                
            //fired
            fire.Invoke();
            hasFired = true;
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
