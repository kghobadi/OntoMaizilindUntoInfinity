using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class Television : MonoBehaviour {
    VideoPlayer vidPlayer;
    public GameObject planes;
    public GameObject sirens;

	void Start () {
        vidPlayer = GetComponent<VideoPlayer>();
	}
	
	void Update () {
		if(vidPlayer.frame >= (long)vidPlayer.frameCount - 3)
        {
            vidPlayer.Stop();
            planes.SetActive(true);
            sirens.SetActive(true);
        }
	}
}
