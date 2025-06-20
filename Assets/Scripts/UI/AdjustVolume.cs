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
    private Vector2 volumeRange = new Vector2(-80f, 20f);
    [SerializeField] private AudioMixer mixer;

    private void Start()
    {
        float trueVal = ExtensionMethods.Remap(Volume, volumeRange.x, volumeRange.y,
            volumeSlider.minValue, volumeSlider.maxValue);
        volumeSlider.value = trueVal;
        SetVolume();
    }

    private void OnEnable()
    {
        float trueVal = ExtensionMethods.Remap(Volume, volumeRange.x, volumeRange.y,
            volumeSlider.minValue, volumeSlider.maxValue);
        volumeSlider.value = trueVal;
        UpdateIFText();
    }

    public void SetVolume()
    {
        float trueVal = ExtensionMethods.Remap(volumeSlider.value, volumeSlider.minValue, volumeSlider.maxValue,
            volumeRange.x, volumeRange.y);
        mixer.SetFloat(volumeValName, trueVal);
        
        PlayerPrefs.SetFloat(volumeValName, trueVal);
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
    
    //TODO will need to create a way for the user to control the slider with a Controller 
    //Once selected from menu selections, Left stick should let you move slider left-right or up-down
}