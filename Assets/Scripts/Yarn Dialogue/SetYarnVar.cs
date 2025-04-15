using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

/// <summary>
/// Simple script for triggering the setting of Yarn variables. 
/// </summary>
public class SetYarnVar : MonoBehaviour
{
    private InMemoryVariableStorage _variableStorage;
    [SerializeField] private string variableName;

    [SerializeField] private bool state;
    [SerializeField] private float value;
    [SerializeField] private string message;

    private void Awake()
    {
        _variableStorage = FindObjectOfType<InMemoryVariableStorage>();
    }

    public void SetYarnBool()
    {
        _variableStorage.SetValue(variableName, state);
    }
    
    public void SetYarnFloat()
    {
        _variableStorage.SetValue(variableName, value);
    }
    
    public void SetYarnString()
    {
        _variableStorage.SetValue(variableName, message);
    }
}
