using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomMaterial : MonoBehaviour {

    public Material[] materialOptions;
    MeshRenderer mRender;
    SkinnedMeshRenderer skinRender;

    void Awake()
    {
        mRender = GetComponent<MeshRenderer>();

        if (mRender == null)
        {
            mRender = GetComponentInChildren<MeshRenderer>();

            if (mRender == null)
            {
                skinRender = GetComponent<SkinnedMeshRenderer>();
            }
        }

    }

    void Start ()
    {
        AssignMaterial(materialOptions);
	}
	
	public void AssignMaterial(Material[] mats)
    {
        int randomMat = Random.Range(0, mats.Length);

        if(mRender)
            mRender.material = mats[randomMat];
        if (skinRender)
            skinRender.material = mats[randomMat];
    }
}
