using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private bool generateOnStart = true;
    [SerializeField] private bool generated;
    [SerializeField] GameObject objectPrefab;
    [SerializeField] int startingNumber = 1000;
    [SerializeField] GameObject[] objects;
    int index;

    public GameObject ObjPrefab => objectPrefab;

    protected virtual void Awake() 
    {
        if (generateOnStart)
        {
            GenerateObjects();
        }  
    }
    
    public virtual void GenerateObjects()
    {
        //make sure to destroy the pool
        if (generated)
        {
            DestroyPool();
        }
        
        // Instantiate all objects.
        objects = new GameObject[startingNumber];
        for (int i = 0; i < objects.Length; i++) 
        {
            objects[i] = InstantiateNew();
            //deactivate
            objects[i].SetActive(false);
        }

        //pool is generated
        generated = true;
    }

    protected virtual GameObject InstantiateNew() 
    {
        GameObject newObject = Instantiate(objectPrefab);
        newObject.transform.parent = transform;
        newObject.AddComponent<PooledObject>();
        newObject.GetComponent<PooledObject>().m_ObjectPooler = this;
        return newObject;
    }

    public virtual GameObject GetObject()
    {
        GameObject nextObject = objects[index];
        index++;
        if (index >= startingNumber)
        {
            index = 0;
        }
        
        return nextObject;
    }

    public virtual GameObject GrabObject(Action beforeEnable = null) 
    {
        GameObject grabbedObject = objects[index];
        index++;
        if (index >= startingNumber)
        {
            index = 0;
        }
        
        //invoke before enable 
        beforeEnable?.Invoke();

        grabbedObject.SetActive(true);

        return grabbedObject;
    }


    public virtual void ReturnObject(GameObject returnedObject) 
    {
        returnedObject.transform.parent = transform;
        returnedObject.SetActive(false);
    }

    /// <summary>
    /// Destroy the pool.
    /// </summary>
    public void DestroyPool()
    {
        //destroy any objs in array 
        if (objects != null)
        {
            foreach (var obj in objects)
            {
                if (obj)
                {
                    DestroyImmediate(obj);
                }
            }
        }

        //loop through and destroy children
        if (transform.childCount > 0)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i))
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                    i--;
                }
            }
        }

        //reset index and nullify objects 
        index = 0;
        objects = null;
        generated = false;
    }
}
