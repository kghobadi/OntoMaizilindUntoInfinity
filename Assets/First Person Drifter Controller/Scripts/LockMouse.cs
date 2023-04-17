// by @torahhorse

using UnityEngine;
using System.Collections;

public class LockMouse : MonoBehaviour
{	
	void Start()
	{
		LockCursor();
	}
	/*
    void Update()
    {
	
    	// lock when mouse is clicked
    	if( Input.GetMouseButtonDown(0) && Time.timeScale > 0.0f )
    	{
    		LockCursor();
    	}
    
    	// unlock when escape is hit
        if  ( Input.GetKeyDown(KeyCode.Escape) && Time.timeScale > 0.0f )
        {
			if (Cursor.lockState == CursorLockMode.Locked){
				UnlockCursor();
			}
			if (Cursor.lockState == CursorLockMode.Confined){
				LockCursor();
			}
        }

    }
    */
    public void UnlockCursor (){
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
	}


    public void LockCursor()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }
}