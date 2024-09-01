using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;
using TMPro;

/// <summary>
/// Allows you to toggle the Global mouse sensitivity for player controllers. 
/// </summary>
public class AdjustMouseSensitivity : MonoBehaviour
{
    [SerializeField] private Slider mouseSensitivity;
    [SerializeField] private TMP_InputField mouseSensitivityIF;
    private void OnEnable()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            mouseSensitivity.value = PlayerPrefs.GetFloat("ControllerSensitivity");
        }
        else
        {
            mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSensitivity");
        }

        UpdateIFText();
    }

    public void SetMouseSensitivity()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            PlayerPrefs.SetFloat("ControllerSensitivity", mouseSensitivity.value);
        }
        else
        {
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity.value);
        }
        UpdateIFText();
    }

    public void SetSlider()
    {
        mouseSensitivity.value = float.Parse(mouseSensitivityIF.text);
    }

    void UpdateIFText()
    {
        float roundedVal = Mathf.Round(mouseSensitivity.value); 
        mouseSensitivityIF.text = roundedVal.ToString();
    }
}
