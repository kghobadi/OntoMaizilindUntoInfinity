using System;
using UnityEngine;
using Yarn;
using Yarn.Unity;

public class VariableInterface : InMemoryVariableStorage
{
    public event System.Action<string> OnVariableUpdated;
    public override void SetValue(string variableName, bool boolValue)
    {
        base.SetValue(variableName, boolValue);
        OnVariableUpdated?.Invoke(variableName);
        PlayerPrefs.SetString(variableName, boolValue.ToString());
    }

    public override void SetValue(string variableName, float floatValue)
    {
        base.SetValue(variableName, floatValue);
        OnVariableUpdated?.Invoke(variableName);
        PlayerPrefs.SetFloat(variableName, floatValue);
    }

    public override void SetValue(string variableName, string stringValue)
    {
        base.SetValue(variableName, stringValue);
        OnVariableUpdated?.Invoke(variableName);
        PlayerPrefs.SetString(variableName, stringValue);
    }

    /// <summary>
    /// Returns string value of a yarn variable. 
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    public string GetValue(string variableName)
    {
        string retVal = "";
        
        if (TryGetValue(variableName, out string stringValue))
        {
            retVal = stringValue;
        }

        //is it still empty? try getting from playerprefs
        if (string.IsNullOrEmpty(retVal))
        {
            retVal = PlayerPrefs.GetString(variableName);
        }
        
        return retVal;
    }
    
    /// <summary>
    /// Returns string value of a yarn variable. 
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    public float GetFloatValue(string variableName)
    {
        return PlayerPrefs.GetFloat(variableName);;
    }

    /// <summary>
    /// Checks the variable against the information stored in variableInterface. Stores the whether or not they match in the doValuesMatch component.
    /// </summary>
    public bool CheckValue (string variableName, string checkedValue)
    {
        string currentValue = null;

        //check against bools, strings, and floats to see if the type is one of those
        if (TryGetValue(variableName, out bool boolValue))
        {
            currentValue = boolValue.ToString().ToLower();
        }
        else if (TryGetValue(variableName, out string stringValue))
        {
            currentValue = stringValue.ToLower();
        }
        else if (TryGetValue(variableName, out float floatValue))
        {
            currentValue = floatValue.ToString().ToLower();
        }

        //check to see if we turned up a value and make sure that value is what we are looking for
        return ((!string.IsNullOrEmpty(currentValue)) && checkedValue == currentValue);

        //Debug.Log(string.Format("currentValue is {0}, the value we are looking for is {1}. Do these values match? {2}", currentValue, checkedValue, doValuesMatch));
    }
}