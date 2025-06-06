﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HalftoneEffect : MonoBehaviour
{
    public Material halfToneMat;    

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, halfToneMat);
    }

}
