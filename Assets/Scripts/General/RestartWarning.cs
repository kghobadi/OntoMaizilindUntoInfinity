using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simply restarts game after restart time. 
/// </summary>
public class RestartWarning : MonoBehaviour
{
    public float restartTime = 10f;
    private float enabledTime;
    private float timeToRestart;
    [SerializeField]
    private TMP_Text text;
    [SerializeField] 
    private string message;
    private void OnEnable()
    {
        enabledTime = Time.fixedTime;
        timeToRestart = enabledTime + restartTime;
    }

    private void FixedUpdate()
    {
        if (gameObject.activeSelf)
        {
            UpdateRestartMessage();
            if (Time.fixedTime > timeToRestart)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void UpdateRestartMessage()
    {
        if (text)
        {
            int time = (int) (timeToRestart - Time.fixedTime);
            text.text = message + " " + time + " seconds";
        }
    }
}
