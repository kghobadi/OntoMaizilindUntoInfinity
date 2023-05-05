using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    //Simply makes an obj persist between scenes.
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
