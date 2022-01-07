using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using InControl;

public class PauseMenu : MonoBehaviour
{
    private AdvanceScene advanceScene;
    private DebugTime debugTime;
    private TitleToRoom title;
    private InputDevice inputDevice;

    public bool paused;
    public UnityEvent toggledPause;
    bool wasTiming;

    public MenuSelections pauseMenu;

    [Tooltip("Anything in this array will pause")]
    public AudioSource[] pauseAudio;
    //temp list to store whatever gets paused. 
    private List<AudioSource> pausedAudios = new List<AudioSource>();

    [Tooltip("Anything in this array will pause")]
    public VideoPlayer[] pauseVideos;

    void Awake ()
    {
        advanceScene = FindObjectOfType<AdvanceScene>();
        debugTime = FindObjectOfType<DebugTime>();
        title = FindObjectOfType<TitleToRoom>();
    }

    private void Start()
    {
        //turn off menu at start 
        if (pauseMenu.gameObject.activeSelf)
        {
            pauseMenu.DeactivateMenu(true);
        }
    }

    void Update ()
    {
        //get input device 
        inputDevice = InputManager.ActiveDevice;
        
        //if you press start or back on controller OR escape on keyboard -- toggle the pause menu.
        if (inputDevice.CommandWasPressed || Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
	}

    /// <summary>
    /// UI prefab friendly pause. 
    /// </summary>
    public void TogglePause()
    {
        if (paused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
        
        toggledPause.Invoke();
    }

    public void Resume()
    {
        //the pause menu is active -- so turn everything off and resume the game
        if (pauseMenu.menuActive)
        {
            pauseMenu.DeactivateAllSubMenus();
            pauseMenu.DeactivateMenu(true);
        
            //set timing 
            Time.timeScale = 1f;

            if (wasTiming)
            {
                debugTime.timing = true;
                wasTiming = false;
            }

            //audio
            for (int i = 0; i < pausedAudios.Count; i++)
            {
                pausedAudios[i].UnPause();
            }

            //video
            for (int i = 0; i < pauseVideos.Length; i++)
            {
                pauseVideos[i].Play();
            }

            paused = false;
        }
        //pause menu not active -- just deactivate all sub menus
        else
        {
            pauseMenu.DeactivateAllSubMenus();
        }
    }
    
    public void Pause()
    {
        //check to see we aren't waiting at Title 
        if(title != null)
        {
            if (title.transitioned)
            {
                Pauses();
            }
        }
        else
        {
            Pauses();
        }
    }

    void Pauses()
    {
        //activate pause menu.
        pauseMenu.gameObject.SetActive(true);
        pauseMenu.ActivateMenu(true);
        //disable interact cursor
        if (InteractCursor.Instance != null)
        {
            InteractCursor.Instance.Deactivate();
        }
        
        //set time
        Time.timeScale = 0f;
        if (debugTime.timing)
        {
            debugTime.timing = false;
            wasTiming = true;
        }
        else
        {
            wasTiming = false;
        }

        //audio pausing
        //clear list
        pausedAudios.Clear();
        for(int i = 0; i < pauseAudio.Length; i++)
        {
            //only pause it if its playing
            if (pauseAudio[i].isPlaying)
            {
                pauseAudio[i].Pause();
                pausedAudios.Add(pauseAudio[i]);
            }
        }
        
        paused = true;

        //video
        if(pauseVideos.Length > 0)
        {
            for (int i = 0; i < pauseVideos.Length; i++)
            {
                pauseVideos[i].Pause();
            }
        }
    }

    /// <summary>
    /// UI prefab friendly restart.
    /// </summary>
    public void Restart()
    {
        advanceScene.Restart();
    }
    
    /// <summary>
    /// UI prefab friendly Quit.
    /// </summary>
    public void Quit()
    {
        advanceScene.Quit();
    }
}
