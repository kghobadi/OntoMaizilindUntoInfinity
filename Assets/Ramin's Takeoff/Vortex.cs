using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Vortex : MonoBehaviour
{
    public Color color1;
    public Color color2;
    public Color color3;
    Renderer rend;
    public float seconds = 2;

    void Start()
    {
        rend = GetComponent<Renderer>();
        FadeTo2();
    }

    void FadeTo1()
    {
        rend.material.DOColor(color1, "_Albedo", seconds).OnComplete(FadeTo2);
    }

    void FadeTo2()
    {
        rend.material.DOColor(color2, "_Albedo", seconds).OnComplete(FadeTo3);
    }

    void FadeTo3()
    {
        rend.material.DOColor(color3, "_Albedo", seconds).OnComplete(FadeTo1);
    }
}
