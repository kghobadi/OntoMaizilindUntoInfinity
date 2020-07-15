using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;
using InControl;

public class MonologueTrigger : MonoBehaviour
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
    [Tooltip("Check to display talking head UI")]
    public bool displayUI;
  
    int activationCount = 0;
    
    //monologues
    [Tooltip("Monologue Managers of the NPCs whose monologues should be activated")]
    public MonologueManager[] myMonologues;
    [Tooltip("Indeces of above Mono Managers to set")]
    public int[] monoNumbers;

    [Header("NPC stuff")]
    [Tooltip("Only need this if the Trigger first becomes active when an NPC moves into it")]
    public GameObject speakerHost;
    [Tooltip("NPC movement system -- generally the same as speaker host?")]
    public Movement npcMovement;
    [Tooltip("Point which Npc will move to to deliver mono")]
    public Transform monologuePoint;
    [Tooltip("How long NPC should wait")]
    public float npcWait = 0;
    [Tooltip("Will attach to NPC upon activation")]
    public bool parentToNPC;

    private void Awake()
    {
        camSwitcher = FindObjectOfType<CameraSwitcher>();
    }

    void OnTriggerEnter(Collider other)
    {
        //player ref 
        CamObject cam = camSwitcher.cameraObjects[camSwitcher.currentCam];
        currentPlayer = cam.gameObject;

        //player entered 
        if (other.gameObject == currentPlayer || other.gameObject.tag == "Player")
        {
            if (!playerInZone && canActivate)
                PlayerEnteredZone();
        }

        //can activate true when speaker arrives 
        if (other.gameObject == speakerHost)
        {
            NPCEnteredZone();
        }
    }

    void OnTriggerStay(Collider other)
    {
        //player ref 
        CamObject cam = camSwitcher.cameraObjects[camSwitcher.currentCam];
        currentPlayer = cam.gameObject;

        if (other.gameObject == currentPlayer || other.gameObject.tag == "Player")
        {
            if (!playerInZone && canActivate)
                PlayerEnteredZone();
        }
    }

    void OnTriggerExit(Collider other)
    {
        //player ref 
        CamObject cam = camSwitcher.cameraObjects[camSwitcher.currentCam];
        currentPlayer = cam.gameObject;

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

            SetNPCWait();
        }
    }

    //NPC will wait to give monologue
    void SetNPCWait()
    {
        if (npcMovement.waitingToGiveMonologue == false)
        {
            //tell npc to go to monologue point 
            if (monologuePoint && activationCount == 0)
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
            //sets monologues 
            for (int i = 0; i < myMonologues.Length; i++)
            {
                myMonologues[i].mTrigger = this;
                myMonologues[i].SetMonologueSystem(monoNumbers[i]);
                myMonologues[i].EnableMonologue();
            }
            
            hasActivated = true;
            activationCount++;
            autoActivate = false;

            //follow NPC 
            if (parentToNPC)
                transform.SetParent(myMonologues[0].transform);
        }
    }
    
    //called in OnTriggerExit()
    public void PlayerExitedZone()
    {
        playerInZone = false;

        if (npcMovement)
        {
            //this is a repeat, so don't wait forever..
            if (activationCount > 0 && npcMovement.GetComponent<MonologueManager>().inMonologue == false)
            {
                npcMovement.waitingToGiveMonologue = false;
                npcMovement.monologueWaitTimer = 0;
                if (npcMovement.waypointCounter > 0)
                    npcMovement.waypointCounter--;
            }
        }
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
