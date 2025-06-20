using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdjustResolution : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private Int32 currentRefreshRate;
    private int currentResolutionIndex = 0;

    private bool fullscreen;
    [SerializeField] private Toggle fullscreenToggle;
    
    void Start()
    {
        //Load fullscreen value from Playerprefs value. 
        if (PlayerPrefs.HasKey("FullScreen"))
        {
            int value = PlayerPrefs.GetInt("FullScreen");
            if (value == 0)
            {
                fullscreen = false;
            }
            else
            {
                fullscreen = true;
            }

            fullscreenToggle.isOn = fullscreen;
        }
        else
        {
            fullscreen = true;
            PlayerPrefs.SetInt("FullScreen", 1);
        }
 
        SetupResolution();
    }

    /// <summary>
    /// Called by UI toggle to turn on/off fullscreen mode. Off == windowed. 
    /// </summary>
    public void ToggleFullScreenMode()
    {
        fullscreen = fullscreenToggle.isOn;
        SetResolution(currentResolutionIndex);
        if (fullscreen)
        {
            PlayerPrefs.SetInt("FullScreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("FullScreen", 0);
        }
    }

    void SetupResolution()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate) 
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        filteredResolutions.Sort((a, b) => {
            if (a.width != b.width)
                return b.width.CompareTo(a.width);
            else
                return b.height.CompareTo(a.height);
        });

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRate + " Hz"; 
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        //update the UI options texts too 
        for(int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            resolutionDropdown.options[i].text = options[i];
        }
        resolutionDropdown.value = currentResolutionIndex = 0;
        resolutionDropdown.RefreshShownValue();
        
        SetResolution(currentResolutionIndex);
    }

    /// <summary>
    /// Allows controllers to cycle through current resolution dropdown list in order. 
    /// </summary>
    public void CycleCurrentResolution()
    {
        if (currentResolutionIndex < resolutionDropdown.options.Count -1)
        {
            currentResolutionIndex++;
        }
        else
        {
            currentResolutionIndex = 0;
        }
        SetResolution(currentResolutionIndex);
    }

    /// <summary>
    /// Sets resolution value from our dropdown list. TODO this should also be remembered across playthroughs with PlayerPrefs? 
    /// </summary>
    /// <param name="resolutionIndex"></param>
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreen);
    }
}