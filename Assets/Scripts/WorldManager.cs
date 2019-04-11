using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public List<GameObject> explosionsToDelete = new List<GameObject>();
    
	
	void Update () {
        //quit app
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //delete all explosion prefabs and 
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            //if there is something in the list 
            if(explosionsToDelete.Count > 0)
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
}
