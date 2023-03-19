using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PooledObject : MonoBehaviour {

    [HideInInspector] public ObjectPooler m_ObjectPooler;
    public UnityEvent grabEvent;
    public UnityEvent returnEvent;

    public void GrabObject()
    {
        grabEvent.Invoke();
    }
    
    public void ReturnToPool() 
    {
        m_ObjectPooler.ReturnObject(gameObject);
        returnEvent.Invoke();
    }
}
