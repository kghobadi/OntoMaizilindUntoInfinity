using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : AudioHandler {

    public ObjectPooler bulletPooler;
    public AudioClip fireWeapon;
	
    public void SpawnBullet()
    {
        PlaySoundRandomPitch(fireWeapon, 1f);

        GameObject bullet = bulletPooler.GrabObject();
        bullet.transform.position = transform.position;
        bullet.GetComponent<Bullet>().shotPos = transform.position;
        bullet.GetComponent<Bullet>().bulletTrail.Clear();
    }
}
