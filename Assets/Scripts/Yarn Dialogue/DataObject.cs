using UnityEngine;

public class DataObject : ScriptableObject
{
    public void Store(string name, float value) => TryStore(name, value.ToString());
    public void Store(string name, bool value) => TryStore(name, value.ToString());
    public void Store(string name, string value) => TryStore(name, value);

    private void TryStore(string name, string value){
        GameObject go = GameObject.FindGameObjectWithTag("Dialogue");
        
        if (go != null && go.TryGetComponent<VariableInterface>(out var vi)) {
            vi.SetValue(name, value);
        }
    }
}