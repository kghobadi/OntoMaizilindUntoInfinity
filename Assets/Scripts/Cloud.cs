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
    float rotateX, rotateY, rotateZ;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        currentSpeed = _cloudGen.cloudSpeed;
        heightFromZero = transform.position.y;
        poolObj = GetComponent<PooledObject>();

        //set rotations
        rotateX = Random.Range(-5f, 5f);
        rotateY = Random.Range(-5f, 5f);
        rotateZ = Random.Range(-5f, 5f);
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
    

}
