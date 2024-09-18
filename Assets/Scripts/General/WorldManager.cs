using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {
    private BombSquadron bomberSquad;
    public List<GameObject> explosionsToDelete = new List<GameObject>();

    public GameObject Bombers => bomberSquad.gameObject;
    private void Awake()
    {
        bomberSquad = FindObjectOfType<BombSquadron>();
    }

    void Start()
    {
        Cursor.visible = false;
    }

    public void DisableAllExplosions()
    {
        //if there is something in the list 
        if (explosionsToDelete.Count > 0)
        {
            for (int i = 0; i < explosionsToDelete.Count; i++)
            {
                explosionsToDelete[i].SetActive(false);
            }
        }
    }

    public void EnableAllExplosions()
    {
        //if there is something in the list 
        if (explosionsToDelete.Count > 0)
        {
            for (int i = 0; i < explosionsToDelete.Count; i++)
            {
                explosionsToDelete[i].SetActive(true);
            }
        }
    }

    public void DeleteAllExplosions()
    {
        //if there is something in the list 
        if (explosionsToDelete.Count > 0)
        {
            for (int i = 0; i < explosionsToDelete.Count; i++)
            {
                Destroy(explosionsToDelete[i]);
            }

            //clear the list immediately
            explosionsToDelete.Clear();
        }
    }
}
