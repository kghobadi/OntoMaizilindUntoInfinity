using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCheck : MonoBehaviour
{
    Transform cam;
    public MeshFilter mFilter;
    public MeshRenderer rend;
    public Mesh lod;
    public Mesh building;
    public float CheckTime = 2;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(Random.value);
        cam = Camera.main.transform;

        InvokeRepeating("CheckDistance", Random.value, CheckTime);

        yield break;
    }

    private void CheckDistance()
    {
        if (Vector3.Distance(cam.position, transform.position) < 150)
        {
            mFilter.mesh = building;
        }
        else
        {
            mFilter.mesh = lod;
        }
    }
}