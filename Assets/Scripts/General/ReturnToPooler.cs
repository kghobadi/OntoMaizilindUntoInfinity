using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPooler : MonoBehaviour {
    PooledObject pooledObj;

    public float waitTime;

    IEnumerator waiter;
    
    void OnEnable()
    {
        if(pooledObj == null)
            pooledObj = GetComponent<PooledObject>();

        StopAllCoroutines();

        if (waiter == null)
        {
            waiter = WaitToReturn();
            StartCoroutine(waiter);
        }
        else
        {
            StartCoroutine(waiter);
        }
    }

    IEnumerator WaitToReturn()
    {
        yield return new WaitForSeconds(waitTime);
        pooledObj.ReturnToPool();
    }
}
