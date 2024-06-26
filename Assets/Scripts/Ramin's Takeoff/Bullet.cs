using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    ThePilot pilot;
    public float bulletSpeed;
    public float speedOverTime = 10f;
    public float origSpeed;

    private Vector3 shotPos;
    private Vector3 targetPos;
    public float shotDist = 500f;
    public PooledObject pooledObj;

    [HideInInspector]
    public TrailRenderer bulletTrail;

    void Awake()
    {
        pilot = FindObjectOfType<ThePilot>();
        bulletTrail = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        pooledObj = GetComponent<PooledObject>();

        bulletSpeed += pilot.moveSpeed;
        origSpeed = bulletSpeed;
    }

    /// <summary>
    /// Guns fire the bullet at the target.
    /// </summary>
    /// <param name="shot"></param>
    /// <param name="target"></param>
    public void ShootBulletAtTarget(Vector3 shot, Vector3 target)
    {
        shotPos = shot; 
        targetPos = target + new Vector3(0, 0, shotDist + 100f);
    }

    void Update () 
    {
        //move forward on Z axis 
        transform.position = Vector3.MoveTowards(transform.position, targetPos, bulletSpeed * Time.deltaTime);
        //Increase speed over time 
        bulletSpeed += speedOverTime;

        //return to pool once it has traveled too far 
        if(Vector3.Distance(transform.position, shotPos) > shotDist)
        {
            ResetBullet();
        }
	}

    void OnTriggerEnter(Collider other)
    {
        //return bullet and death cloud to their pools on impact 
        if(other.gameObject.CompareTag("DeathCloud"))
        {
            //reset cloud scale && send to poolers
            //other.gameObject.transform.localScale = other.gameObject.GetComponent<Cloud>().origScale;
            //other.gameObject.GetComponent<PooledObject>().ReturnToPool();
        }
        
        //return bullet and death cloud to their pools on impact 
        if(other.gameObject.CompareTag("Deity"))
        {
            //nothing because ResetBullet is called by Deity. 
        }
    }

    //can be called by Deities
    public void ResetBullet(Transform Deity = null)
    {
        pooledObj.ReturnToPool();
        bulletSpeed = origSpeed;
        bulletTrail.Clear();

        //lock on check for the Deity transforms 
        if (Deity != null && pilot.useLockOnTargeting)
        {
            pilot.LockOnToTarget(Deity);
        }
    }
}
