using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public List<GameObject> explosionsToDelete = new List<GameObject>();

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
