using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PauseMenu : MonoBehaviour {
    DebugTime debugTime;
    TitleToRoom title;

    public bool paused;
    bool wasTiming;

    public GameObject pauseMenu;

    [Tooltip("Anything in this array will pause")]
    public AudioSource[] pauseAudio;

    [Tooltip("Anything in this array will pause")]
    public VideoPlayer[] pauseVideos;

    void Awake ()
    {
        debugTime = FindObjectOfType<DebugTime>();
        title = FindObjectOfType<TitleToRoom>();

        //turn off menu at start 
        if (pauseMenu.activeSelf)
            pauseMenu.SetActive(false);
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
	}

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
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;

        //set timing 
        Time.timeScale = 1f;

        if (wasTiming)
        {
            debugTime.timing = true;
            wasTiming = false;
        }

        //audio
        for (int i = 0; i < pauseAudio.Length; i++)
        {
            pauseAudio[i].UnPause();
        }

        //video
        for (int i = 0; i < pauseVideos.Length; i++)
        {
            pauseVideos[i].Play();
        }

        paused = false;
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
        pauseMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

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

        //audio
        for(int i = 0; i < pauseAudio.Length; i++)
        {
            pauseAudio[i].Pause();
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
}
