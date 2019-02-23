using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    //general
    public bool hasActivated;

    //dialogue
    public DialogueText[] myDialogues;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!hasActivated)
            {
                for (int i = 0; i < myDialogues.Length; i++)
                {
                    myDialogues[i].EnableDialogue();
                }

                hasActivated = true;
            }
        }
    }

    void OnEnable()
    {
        hasActivated = false;
    }

    void OnDisable()
    {
        if (hasActivated)
        {
            for (int i = 0; i < myDialogues.Length; i++)
            {
                myDialogues[i].DisableDialogue();
            }
        }
       
    }

}
