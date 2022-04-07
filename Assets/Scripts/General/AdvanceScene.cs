using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;
using UnityEngine.Events;

public class AdvanceScene : MonoBehaviour 
{
    public float timeToRestart = 5f;
    public float restartTimer;

    public UnityEvent onSceneLoad;
    public bool debug;

	void Update ()
    {
        if (debug)
        {
            //shift scenes
            SceneShifting();

            //restart game
            HoldToRestart();
        }
    }

    void SceneShifting()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //next scene 
        if (Input.GetKeyDown(KeyCode.Return) || inputDevice.RightBumper.WasPressed)
        {
            LoadNextScene();
        }

        //previous scene 
        if (Input.GetKeyDown(KeyCode.CapsLock) || inputDevice.LeftBumper.WasPressed)
        {
            LoadPreviousScene();
        }
    }

    void HoldToRestart()
    { 
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        if (Input.GetKey(KeyCode.Delete) || (inputDevice.Command))
        {
            restartTimer += Time.deltaTime;

            if (restartTimer > timeToRestart)
            {
                Restart();
            }
        }
        else
        {
            restartTimer = 0;
        }
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        
        onSceneLoad.Invoke();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
        onSceneLoad.Invoke();
    }

    //waits to load 
    public void WaitToLoadNextScene(float time)
    {
        StartCoroutine(WaitToLoad(time));
    }

    IEnumerator WaitToLoad(float wait)
    {
        yield return new WaitForSeconds(wait);

        LoadNextScene();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        
        onSceneLoad.Invoke();

        Debug.Log("Restart");
    }

    public void Quit()
    {
        Application.Quit();

        Debug.Log("QUIT");
    }
}
