using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;
using TMPro;
using UnityEngine.Audio;

/// <summary>
/// Allows you to toggle the Global volume values. 
/// </summary>
public class AdjustVolume : MonoBehaviour
{
    private float Volume
    {
        get
        {
            float vol = 0f;
            if (PlayerPrefs.HasKey(volumeValName))
            {
                vol = PlayerPrefs.GetFloat(volumeValName);
            }
            else
            {
                PlayerPrefs.SetFloat(volumeValName, vol);
            }

            return vol;
        }
    }
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_InputField volumeIF;
    [SerializeField] private string volumeValName = "masterVol";
    [SerializeField] private AudioMixer mixer;

    private void Start()
    {
        volumeSlider.value = Volume;
        SetVolume();
    }

    private void OnEnable()
    {
        volumeSlider.value = Volume;
        UpdateIFText();
    }

    public void SetVolume()
    {
        mixer.SetFloat(volumeValName, volumeSlider.value);
        PlayerPrefs.SetFloat(volumeValName, volumeSlider.value);
        UpdateIFText();
    }

    public void SetSlider()
    {
        volumeSlider.value = float.Parse(volumeIF.text);
    }

    void UpdateIFText()
    {
        float roundedVal = Mathf.Round(volumeSlider.value); 
        volumeIF.text = roundedVal.ToString();
    }
}