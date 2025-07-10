using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    GameObject _player;

    float currentSpeed;
    public CloudGenerator _cloudGen;
    
    PooledObject poolObj;

    public float heightFromZero;
    public float rotationMin = -3f, rotationMax = 3f;
    float rotateX, rotateY, rotateZ;
    private Vector3 origScale;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        origScale = transform.localScale;
    }

    void Start()
    {
        currentSpeed = _cloudGen.cloudSpeed;
        heightFromZero = transform.position.y;
        poolObj = GetComponent<PooledObject>();

        RandomizeRotations();
    }

    //set rotation speeds
    void RandomizeRotations()
    {
        if (rotationMin != 0 && rotationMax != 0)
        {
            rotateX = Random.Range(rotationMin, rotationMax);
            rotateY = Random.Range(rotationMin, rotationMax);
            rotateZ = Random.Range(rotationMin, rotationMax);
        }
    }
    
    void Update()
    {
        //move at current speed
        transform.position = Vector3.MoveTowards(transform.position,
           transform.position - new Vector3(0, 0, 10), currentSpeed * Time.deltaTime);
        //rotate
        transform.Rotate(rotateX, rotateY, rotateZ);

        //destroy when it gets far away enough from original generator
        if (Mathf.Abs(transform.position.z - _cloudGen.transform.position.z) > _cloudGen.distanceToDestroy)
        {
            transform.localScale = origScale;
            poolObj.ReturnToPool();
        }
        
        AdjustHeight();
    }
    
    //adjusts height based on raycast at the Ground (terrain)
    void AdjustHeight()
    {
        Vector3 down = transform.TransformDirection(Vector3.down) * 10;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, down, out hit, 500))
        {
            transform.position = Vector3.MoveTowards(transform.position, hit.point + new Vector3(0, heightFromZero, 0), 5 * Time.deltaTime);
        }
    }

    public void ResetScale()
    {
        transform.localScale = origScale;
    }
}
