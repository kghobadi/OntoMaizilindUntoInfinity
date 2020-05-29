using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//allows you to speed up time scale 
public class DebugTime : MonoBehaviour {
    public bool debug;
    public float speedUp = 10f;
    
    void Update ()
    {
        if (debug)
        {
            if (Input.GetKey(KeyCode.Minus))
            {
                Time.timeScale = speedUp;
            }
            else if(Input.GetKeyUp(KeyCode.Minus))
            {
                Time.timeScale = 1f;
            }
        }
	}

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
