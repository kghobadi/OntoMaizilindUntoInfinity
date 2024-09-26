using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Allows additional UI element to assist in selection. 
/// </summary>
public class MenuSelectionExtender : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private MenuSelections _menuSelections;
    [SerializeField] private Selectors selection;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _menuSelections.SetMenuSelection(selection);
    }
}
