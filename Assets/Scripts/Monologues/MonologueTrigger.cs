using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;
using InControl;

public class MonologueTrigger : TriggerBase
{
    //player refs
    GameObject currentPlayer;
    CameraSwitcher camSwitcher;

    //general
    public string playerTag = "Human";

    [Tooltip("Defaults to true, uncheck if player can only activate once an NPC enters it")]
    public bool canActivate = true;
    [Tooltip("Check to auto activate when player enters trigger")]
    public bool autoActivate;
    [Tooltip("True when monologue has been activated")]
    public bool hasActivated;
    [Tooltip("True when player is within trigger")]
    public bool playerInZone;
    
    //monologues
    [Tooltip("Monologue Managers of the NPCs whose monologues should be activated")]
    public MonologueManager[] myMonologues;
    [Tooltip("Indeces of above Mono Managers to set")]
    public int[] monoNumbers;

    [Header("NPC stuff")]
    [Tooltip("Check to make it only activate by speaker host NPC.")]
    public bool mustBeSpeakerHost;
    [Tooltip("Only need this if the Trigger first becomes active when an NPC moves into it")]
    public GameObject speakerHost;
    [Tooltip("NPC movement system -- generally the same as speaker host?")]
    public Movement npcMovement;
    [Tooltip("Point which Npc will move to to deliver mono")]
    public Transform monologuePoint;
    [Tooltip("How long NPC should wait")]
    public float npcWait = 0;

    private void Awake()
    {
        camSwitcher = FindObjectOfType<CameraSwitcher>();
    }

    void OnTriggerEnter(Collider other)
    {
        //player ref 
        if (camSwitcher)
        {
            currentPlayer = camSwitcher.currentPlayer;
        }

        //player entered 
        if (other.gameObject == currentPlayer || other.gameObject.tag == "Player")
        {
            if (!playerInZone && canActivate)
                PlayerEnteredZone();
        }

        //npc entered trigger -- activate 
        if (other.gameObject == speakerHost && mustBeSpeakerHost && canActivate == false)
        {
            NPCEnteredZone();
        }
        else if (!mustBeSpeakerHost && other.gameObject.tag == "Human" && canActivate == false)
        {
            NPCEnteredZone();
        }
    }

    void OnTriggerStay(Collider other)
    {
        //player ref 
        if (camSwitcher)
        {
            currentPlayer = camSwitcher.currentPlayer;
        }
        
        if (other.gameObject == currentPlayer || other.gameObject.tag == "Player")
        {
            if (!playerInZone && canActivate)
                PlayerEnteredZone();
        }

        //npc entered trigger -- activate 
        if (other.gameObject == speakerHost && mustBeSpeakerHost && canActivate == false)
        {
            NPCEnteredZone();
        }
        else if (!mustBeSpeakerHost && other.gameObject.tag == "Human" && canActivate == false)
        {
            NPCEnteredZone();
        }
    }

    void OnTriggerExit(Collider other)
    {
        //player ref 
        if (camSwitcher)
        {
            currentPlayer = camSwitcher.currentPlayer;
        }

        if (other.gameObject == currentPlayer || other.gameObject.tag == "Player")
        {
            PlayerExitedZone();
        }
    }

    void Update()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        if (playerInZone)
        {
            if ((Input.GetKeyUp(KeyCode.Space) || inputDevice.Action3.WasPressed || autoActivate) && !hasActivated)
            {
                ActivateMonologue();
            }
        }
    }

    //called in OnTriggerEnter()
    public void PlayerEnteredZone()
    {
        if (!hasActivated)
        {
            playerInZone = true;

            if(npcMovement)
                SetNPCWait();
        }
    }

    void NPCEnteredZone()
    {
        if (!hasActivated)
        {
            //Debug.Log("can activate!");
            canActivate = true;

            //npc just begins monologue 
            if (autoActivate)
            {
                ActivateMonologue();
            }
        }
    }

    //NPC will wait to give monologue
    void SetNPCWait()
    {
        if (npcMovement.waitingToGiveMonologue == false)
        {
            //tell npc to go to monologue point 
            if (monologuePoint)
            {
                npcMovement.SetIdle();
                npcMovement.NavigateToPoint(monologuePoint.position, true);
            }
            //wait to give monologue when you arrive 
            else
            {
                npcMovement.SetIdle();
                npcMovement.waitingToGiveMonologue = true;
            }
        }
    }

    //activates monologues
    void ActivateMonologue()
    {
        if (!hasActivated)
        {
            //sets monologues -- should have a check to wait until the character finishes their current monologue, if active. 
            for (int i = 0; i < myMonologues.Length; i++)
            {
                if (myMonologues[i].inMonologue)
                {
                    myMonologues[i].mTrigger = this;
                    myMonologues[i].WaitToSetNewMonologue(monoNumbers[i]);
                }
                else
                {
                    myMonologues[i].mTrigger = this;
                    myMonologues[i].SetMonologueSystem(monoNumbers[i]);
                    myMonologues[i].EnableMonologue();
                }
            }
            
            hasActivated = true;
            autoActivate = false;
        }
    }
    
    //called in OnTriggerExit()
    public void PlayerExitedZone()
    {
        playerInZone = false;
    }

    //called when monologue text script is reset
    public void WaitToReset(float time)
    {
        StartCoroutine(WaitToReactivate(time));
    }

    IEnumerator WaitToReactivate(float timer)
    {
        yield return new WaitForSeconds(timer);

        hasActivated = false;
    }
}
