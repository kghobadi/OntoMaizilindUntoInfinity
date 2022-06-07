using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSkybox : MonoBehaviour
{
    public Material newSky;

    public void SetSky()
    {
        RenderSettings.skybox = newSky;
    }
}
