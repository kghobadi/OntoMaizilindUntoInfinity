using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakingCharacter : DialogueSource
{
    public string CharacterName{ get => characterName; }
    
    public SpeechBubbleParameters SpeechBubbleParameters{ get => speechBubbleParameters; set => speechBubbleParameters = value; }
    public Vector2 BubbleOffset{ get => bubbleOffset; }
    public Vector2 TailTipPosition{ get => tailTipPosition; }
    
    [SerializeField]
    private string characterName;

    [Header("Bubble")]
    [SerializeField]
    private SpeechBubbleParameters speechBubbleParameters;
    [SerializeField]
    private Vector2 bubbleOffset;
    [SerializeField]
    private Vector2 tailTipPosition;

    [Space(10)]
    [SerializeField]
    private SpeakingCharacterList speakingCharacterList;
    
    #if UNITY_EDITOR
    // Called when added to game object
    private void Reset() 
    {
        characterName = this.name;
    }
    #endif

    private void OnEnable() 
    {
        speakingCharacterList?.Add(this);
    }

    private void OnDisable() 
    {
        speakingCharacterList?.Remove(this);
    }

    public void SetNode(string newNode)
    {
        CurrentDialogueNode = newNode;
    }

    public override void BeginDialogue()
    {
        //Events.TalkToNPC.Raise(this, new StringEventArgs(characterName));
       
       
        base.BeginDialogue();
    }

    private void OnDrawGizmos() 
    {
        // TODO: gizmo visualization of tailtippos
        // Copy draggable node code from collidertooleditor
    }
}