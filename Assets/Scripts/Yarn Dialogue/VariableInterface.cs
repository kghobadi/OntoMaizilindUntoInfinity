using UnityEngine;
using Yarn;
using Yarn.Unity;

// PROTO TODO: Decouple variable storage from Dialogue prefab or attach dialogue prefab to game manager
public class VariableInterface : InMemoryVariableStorage
{
    public event System.Action<string> OnVariableUpdated;

    public override void SetValue(string variableName, bool boolValue)
    {
        base.SetValue(variableName, boolValue);
        OnVariableUpdated?.Invoke(variableName);
    }

    public override void SetValue(string variableName, float floatValue)
    {
        base.SetValue(variableName, floatValue);
        OnVariableUpdated?.Invoke(variableName);
    }

    public override void SetValue(string variableName, string stringValue)
    {
        base.SetValue(variableName, stringValue);
        OnVariableUpdated?.Invoke(variableName);
    }
}