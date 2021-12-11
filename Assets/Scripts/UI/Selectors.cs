using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectors : MonoBehaviour 
{
    [HideInInspector]
    public MenuSelections menuSelections;
    public bool deactivateMainMenu;

    public bool canBeLocked;
    public bool unlocked;

    public GameObject[] selectorsActive;
    public UnityEvent[] selectionEvents;

    void Awake()
    {
        if (menuSelections == null)
            menuSelections = GetComponentInParent<MenuSelections>();
    }

    private void OnEnable()
    {
        DeactivateSelectors();
    }

    public void ActivateSelectors()
    {
        //play sound from menu
        if (menuSelections)
        {
            if (menuSelections.selects.Length > 0)
            {
                menuSelections.PlayRandomSound(menuSelections.changeSelections, 1f);
            }
        }

        if (canBeLocked)
        {
            if (unlocked)
            {
                for (int i = 0; i < selectorsActive.Length; i++)
                {
                    selectorsActive[i].SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < selectorsActive.Length; i++)
            {
                selectorsActive[i].SetActive(true);
            }
        }

        DeactivateOtherSelectors();
    }

    void DeactivateOtherSelectors()
    {
        for (int i = 0; i < menuSelections.menuSelections.Count; i++)
        {
            if (menuSelections.menuSelections[i] != this)
            {
                menuSelections.menuSelections[i].DeactivateSelectors();
            }
        }
    }

    public void DeactivateSelectors()
    {
        for (int i = 0; i < selectorsActive.Length; i++)
        {
            selectorsActive[i].SetActive(false);
        }
    }

   
    public void SelectMe()
    {
        //play sound from menu
        if (menuSelections)
        {
            if (menuSelections.selects.Length > 0)
            {
                menuSelections.PlayRandomSound(menuSelections.selects, 1f);
            }
        }

        //call the selection events 
        for (int i = 0; i < selectionEvents.Length; i++)
        {
            selectionEvents[i].Invoke();
        }

        if (deactivateMainMenu)
        {
            //check if submenus are active
            if (menuSelections.CheckSubMenusActive())
            {
                //disable menu selections while submenu is active
                menuSelections.DeactivateMenu(false);
            }
        }
    }
}