using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeList<T> : ScriptableObject, IEnumerable<T>
{
    public List<T> Items {get => items;}
    private List<T> items = new List<T>();

    public void Add(T item) {
        if (!items.Contains(item))
            items.Add(item);
    }

    public void Remove(T item) {
        if (items.Contains(item))
            items.Remove(item);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)items).GetEnumerator();
    }
}