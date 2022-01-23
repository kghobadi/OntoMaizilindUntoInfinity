using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

// DialogueHaver1998 has logged on
public class DialogueSource : MonoBehaviour
{
    public string CurrentDialogueNode 
    {   
        get => dialogueNode;
        set => dialogueNode = value;
    }

    [Header("Dialogue")]
    [SerializeField]
    private string dialogueNode;

    private UnityEngine.Events.UnityEvent<string> playDialogueEventString;

    protected virtual void Awake()
    {
        playDialogueEventString = EventMgr.Instance.CreateOrGetEventSink<string>(EventSinks.Dialogue.PlayDialogue);
    }

    public virtual void BeginDialogue()
    {
        playDialogueEventString?.Invoke(dialogueNode);
    }

    //TODO look into this and see if its needed
    /*
    // Changes current dialogue node
    [YarnCommand("node")]
    public virtual void SetNode(string name){
        dialogueNode = name;
    }
    */
}