using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : AudioHandler {

    public ObjectPooler bulletPooler;
    public AudioClip fireWeapon;

    public Transform floatingTarget;
	
    public void SpawnBullet()
    {
        PlaySoundRandomPitch(fireWeapon, 1f);

        GameObject bullet = bulletPooler.GrabObject();
        bullet.transform.position = transform.position;
        Vector3 shot = new Vector3(transform.position.x, floatingTarget.position.y, transform.position.z);
        bullet.GetComponent<Bullet>().shotPos = shot;
        bullet.GetComponent<Bullet>().bulletTrail.Clear();
    }
}
