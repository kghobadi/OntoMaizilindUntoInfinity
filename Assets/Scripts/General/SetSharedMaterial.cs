using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple component to set and override a shared mat of a mesh renderer at runtime. 
/// </summary>
public class SetSharedMaterial : MonoBehaviour
{
   [SerializeField] private MeshRenderer mr;
   [SerializeField] private Material material;
   private Material[] origMats;

   private void Start()
   {
      origMats = mr.materials;
   }
   
   public void SetOverrideSharedMat(int index) => OverrideSharedMaterial(index, material);
   
   public void OverrideSharedMaterial(int index, Material sharedMat)
   {
      Material[] mats = mr.materials;
      mats[index] = sharedMat;
      mr.materials = mats;
   }

   public void ReturnToOriginalMaterials()
   {
      mr.materials = origMats;
   }
}
