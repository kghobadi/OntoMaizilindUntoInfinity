﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTrigger : MonoBehaviour {


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Plane")
        {
            Bomber bomber = other.gameObject.GetComponent<Bomber>();

            bomber.DropBombs();
            Debug.Log("triggering bombs");
        }
    }
}