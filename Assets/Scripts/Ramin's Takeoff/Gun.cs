using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : AudioHandler {

    public ObjectPooler bulletPooler;
    public AudioClip fireWeapon;

    public Transform floatingTarget;
	
    /// <summary>
    /// L mouse spawns these from ThePilot 
    /// </summary>
    public void SpawnBullet()
    {
        PlaySoundRandomPitch(fireWeapon, 1f);

        GameObject bullet = bulletPooler.GrabObject();
        bullet.transform.position = transform.position;
        //TODO beginning shot pos this way doesn't allow us to target different spots at all. 
        //We would need to make shooting about setting a destination from the starting point based on where floating target is 
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.ShootBulletAtTarget(transform.position, floatingTarget.position);
        bulletScript.bulletTrail.Clear();
    }

    //TODO can create missle system here that uses right click
    public void SpawnMissle()
    {
        // sound
        //create pooler and effect for it.
        // should be way faster and bigger than bullet 
    }
}
