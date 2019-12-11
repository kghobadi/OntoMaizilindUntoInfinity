using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this scripts stores info about what kind of camera object we are switching too
//will all me to access various kinds of movement scripts, camera, and body of obj
public class CamObject : MonoBehaviour {

    public CamType myCamType;
    public Camera camObj;
    public GameObject headset, myBody;

    public enum CamType
    {
        HUMAN, BOMBER,
    }
    
}
