using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : AudioHandler
{
    [Header("Sounds")]
    public AudioClip startSong;
    public AudioClip shahSpeech;
    public AudioClip staticBroadcast;

    private void Start()
    {
        //disable on start 
        gameObject.SetActive(false);
    }
}
