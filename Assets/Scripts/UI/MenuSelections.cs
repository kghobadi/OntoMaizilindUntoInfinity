using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

//this script allows you to move around menus with a controller
public class MenuSelections : AudioHandler
{
    [Header("Menu Sounds")]
    public AudioClip[] changeSelections;
    public AudioClip[] selects;

    [Header("Menu Selections")]
    public bool menuActive;
    public List<Selectors> menuSelections = new List<Selectors>();
    public MenuSelections[] subMenus;
    public int currentSelector = 0;

    //for controller selections 
    bool canChange;
    //counted in frames because time is Stopped in pause.
    private int changeTimer;
    public int changeReset = 30;
    InputDevice inputDevice;

    private void Start()
    {
        //assign within star selectors 
        for(int i = 0; i < menuSelections.Count; i++)
        {
            menuSelections[i].menuSelections = this;
        }
    }

    void Update ()
    {
        //get input device 
        inputDevice = InputManager.ActiveDevice;

        if (menuActive)
        {
            //handles controller inputs for the menu
            ControlSelections();
        }
        
        //resets when you change selectors
        ChangeReset();

        //detection of closing submenus
        if (inputDevice.Action2.WasPressed)
        {
            DeactivateAllSubMenus();
        }
    }

    //activates menu and resets selection
    public void ActivateMenu(bool enableObject)
    {
        if (enableObject)
        {
            gameObject.SetActive(true);
        }
        
        menuActive = true;

        //deactivate all stars 
        for (int i = 0; i < menuSelections.Count; i++)
        {
            menuSelections[i].DeactivateSelectors();
        }

        //reset to selector 0
        currentSelector = 0;
        if (menuSelections.Count > 0)
            menuSelections[currentSelector].ActivateSelectors();
        canChange = true;

        //No lock state in menus
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void DeactivateMenu(bool disableObject)
    {
        menuActive = false;
        if (disableObject)
        {
            gameObject.SetActive(false);
        }
        //Debug.Log("deactivated" + gameObject.name);
    }

    //function controls selection with controller
    void ControlSelections()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;
        
        //create input Y
        float inputValY = 0f;
        
        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            //get inputY
            inputValY = inputDevice.LeftStickY;
        }
        //keyboard
        else
        {
            //up
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) )
            {
                inputValY = 1f;
            }
            //down
            else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                inputValY = -1f;
            }
        }

        //if we have no input then set can change true again.
        if (inputValY == 0)
        {
            SetCanChange();
        }

        //detection of changing 
        if (canChange)
        {
            //pos val, selection moves down
            if (inputValY > 0)
            {
                ChangeMenuSelector(false);
            }
            //neg val, selection moves up
            else if (inputValY < 0)
            {
                ChangeMenuSelector(true);
            }
        }

        //detection of selection
        if (inputDevice.Action1.WasPressed || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (menuSelections[currentSelector])
                menuSelections[currentSelector].SelectMe();
        }
    }

    //allows you to increment menu selector & change stars
    public void ChangeMenuSelector(bool upOrDown)
    {
        if (menuSelections.Count > 1)
        {
            //deactivate current stars
            menuSelections[currentSelector].DeactivateSelectors();

            //up
            if (upOrDown)
            {
                //increment up
                if (currentSelector < menuSelections.Count - 1)
                {
                    currentSelector++;
                }
                else
                {
                    currentSelector = 0;
                }
            }
            //down
            else
            {
                //increment down
                if (currentSelector > 0)
                {
                    currentSelector--;
                }
                else
                {
                    currentSelector = menuSelections.Count - 1;
                }
            }

            //activate next stars
            menuSelections[currentSelector].ActivateSelectors();

            //change reset called 
            canChange = false;
        }
    }

    public void SetMenuSelection(Selectors selection) => SetMenuSelection(menuSelections.IndexOf(selection));

    /// <summary>
    /// Mouse hover calls this 
    /// </summary>
    /// <param name="index"></param>
    public void SetMenuSelection(int index)
    {
        if (menuSelections.Count > 1 && index != currentSelector)
        {
            //deactivate current stars
            menuSelections[currentSelector].DeactivateSelectors();

            //must be within selections range 
            if (index >= 0 && index <= menuSelections.Count - 1)
            {
                //update index
                currentSelector = index;
                //activate next stars
                menuSelections[currentSelector].ActivateSelectors();
            }
        }
    }

    //handles reset timer for controller selection
    void ChangeReset()
    {
        if (canChange == false)
        {
            //add a frame
            changeTimer += 1;

            //time to reset?
            if (changeTimer > changeReset)
            {
                SetCanChange();
            }
        }
    }

    void SetCanChange()
    {
        canChange = true;
        changeTimer = 0;
    }

    //turns on all menu selections
    public void ActivateSelections()
    {
        for (int i = 0; i< menuSelections.Count; i++)
        {
            menuSelections[i].gameObject.SetActive(true);
        }
    }

    //turns off all menu selections
    public void DeactivateSelections()
    {
        for (int i = 0; i < menuSelections.Count; i++)
        {
            menuSelections[i].gameObject.SetActive(false);
        }

        //Debug.Log("deactivated" + gameObject.name + " selections");
    }
    
    public void AddMenuSelection(Selectors selector)
    {
        if (menuSelections.Contains(selector))
        {
            Debug.Log("Menu already contains that selector");
            return;
        }
        
        menuSelections.Add(selector);
        if (menuActive)
        {
            selector.gameObject.SetActive(true);
        }
    }
    
    public void AddMenuSelectionAtIndex(Selectors selector, int index)
    {
        if (menuSelections.Contains(selector))
        {
            Debug.Log("Menu already contains that selector");
            return;
        }
        
        menuSelections.Insert(index, selector);
        if (menuActive)
        {
            selector.gameObject.SetActive(true);
        }
    }

    public void RemoveMenuSelectionAtIndex(int index)
    {
        Selectors selector = menuSelections[index];
        menuSelections.RemoveAt(index);
        selector.gameObject.SetActive(false);
        
        //move menu selector down in list 
        ChangeMenuSelector(false);
    }
    
    public void RemoveMenuSelection(Selectors selector)
    {
        if (menuSelections.Contains(selector) == false)
        {
            Debug.Log("Menu does not contain that selector");
            return;
        }
        
        menuSelections.Remove(selector);
        selector.gameObject.SetActive(false);
    }


    //turns off all subMenus
    public void DeactivateAllSubMenus()
    {
        for (int i = 0; i < subMenus.Length; i++)
        {
            subMenus[i].DeactivateAllSubMenus();
            subMenus[i].DeactivateMenu(true);
        }

        //reactivate menu if it was disabled by opening a submenu
        if(menuActive == false)
        {
            ActivateMenu(true);
        }
    }

    //checks if there is a sub menu open
    public bool CheckSubMenusActive()
    {
        for (int i = 0; i < subMenus.Length; i++)
        {
            if (subMenus[i].gameObject.activeSelf)
                return true;
        }

        return false;
    }
}