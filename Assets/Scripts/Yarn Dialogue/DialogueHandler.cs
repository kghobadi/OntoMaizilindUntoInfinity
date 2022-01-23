using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class DialogueHandler: DialogueViewBase
{
    // TODO: this should maybe be a dropdown or enforced in some other way?
    public string playerSpeakerTag = "Player";

    [SerializeField] SpeechBubbleController speechBubbleController;
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] SpeakingCharacterList speakingCharacters;

    private UnityEvent onDialogueStartEvent;
    private UnityEvent onDialogueEndEvent;
    private SpeakingCharacter playerSpeaker;

    private List<DialogueOption> currentOptions = null;
    private Action<int> onOptionSelected;
    private int currentOptionIndex;

    private bool IsShowingOptions => currentOptions?.Count > 0;

    [Header("Hack, please remove later")]
    public float minSpamTime = 0.25f;
    private float timer = 0f;

    public DialogueRunner DialogueRunner
    {
        get
        {
            return dialogueRunner;
        }
    }

    private void Update ()
    {
        timer += Time.deltaTime;
    }

    void Start ()
    {
        onDialogueStartEvent = EventMgr.Instance.CreateOrGetEventSink(EventSinks.Dialogue.OnStart);
        onDialogueEndEvent = EventMgr.Instance.CreateOrGetEventSink(EventSinks.Dialogue.OnEnd);
    }

    private void OnEnable ()
    {
        EventMgr.Instance.Subscribe<string>(EventSinks.Dialogue.PlayDialogue, OnPlayDialogue);
        EventMgr.Instance.Subscribe(EventSinks.Dialogue.OnAdvance, OnAdvance);
        EventMgr.Instance.Subscribe(EventSinks.Dialogue.NextChoice, OnNextChoice);
        EventMgr.Instance.Subscribe(EventSinks.Dialogue.PrevChoice, OnPrevChoice);
    }

    private void OnDisable ()
    {
        EventMgr.Instance?.Unsubscribe<string>(EventSinks.Dialogue.PlayDialogue, OnPlayDialogue);
        EventMgr.Instance?.Unsubscribe(EventSinks.Dialogue.OnAdvance, OnAdvance);
        EventMgr.Instance?.Unsubscribe(EventSinks.Dialogue.NextChoice, OnNextChoice);
        EventMgr.Instance?.Unsubscribe(EventSinks.Dialogue.PrevChoice, OnPrevChoice);
    }

    private void OnAdvance ()
    {
        //UGLY HACK TO PREVENT SPAMMING FOR NOW
        if (timer > minSpamTime)
        {
            if (IsShowingOptions)
            {
                //even uglier hack to prevent further spamming on option selection
                if (timer > minSpamTime * 2)
                {
                    onOptionSelected(currentOptions[currentOptionIndex].DialogueOptionID);
                    currentOptions.Clear();
                }
            }
            else
            {
                dialogueRunner.OnViewUserIntentNextLine();
            }

            timer = 0.0f;
        }
    }

    private void OnNextChoice ()
    {
        if (IsShowingOptions == false)
        {
            return;
        }
        this.currentOptionIndex = (this.currentOptionIndex + 1) % this.currentOptions.Count;
        ShowCurrentOption();
    }

    private void OnPrevChoice ()
    {
        if (IsShowingOptions == false)
        {
            return;
        }

        this.currentOptionIndex = this.currentOptionIndex - 1;
        if (this.currentOptionIndex < 0)
        {
            this.currentOptionIndex += this.currentOptions.Count;
        }

        ShowCurrentOption();
    }

    private void OnPlayDialogue (string nodeName)
    {
        dialogueRunner.StartDialogue(nodeName);
    }

    /// <summary>
    /// Public method for starting a dialogue. 
    /// </summary>
    /// <param name="node"></param>
    public void PlayDialogue(string node)
    {
        OnPlayDialogue(node);
    }

    public override void DialogueStarted ()
    {
        onDialogueStartEvent?.Invoke();
    }

    public override void DialogueComplete ()
    {
        // Close the bubble, if it's still visible
        speechBubbleController.CloseBubble();

        if (onDialogueEndEvent != null)
        {
            onDialogueEndEvent.Invoke();
        }

    }

    public override void RunLine (LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        var speakerName = dialogueLine.CharacterName;

        if (string.IsNullOrEmpty(speakerName))
        {
            Debug.LogWarning($@"Not running line ""{dialogueLine.Text.Text}"" because it doesn't have a speaker name.");
            onDialogueLineFinished();
            return;
        }

        if (speakingCharacters.TryGetCharacter(speakerName, out var speaker) == false)
        {
            Debug.LogError($@"Not running line ""{dialogueLine.Text.Text}"", because a {nameof(SpeakingCharacter)} named {speakerName} could not be found.");
            onDialogueLineFinished();
            return;
        }

        speechBubbleController.SpeechBubbleParameters = speaker.SpeechBubbleParameters;

        speechBubbleController.ShowBubble(speaker.transform, dialogueLine.TextWithoutCharacterName.Text, speaker.BubbleOffset, speaker.TailTipPosition, onComplete: onDialogueLineFinished);
    }

    public override void RunOptions (DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        // Options are always presented from the Player, so we don't need
        // to do a lookup for the SpeakingCharacter to present the bubble
        // from
        this.currentOptions = new List<DialogueOption>(dialogueOptions);
        this.onOptionSelected = onOptionSelected;
        this.currentOptionIndex = 0;
        ShowCurrentOption();
    }

    private void ShowCurrentOption ()
    {
        if (playerSpeaker == null)
        {
            playerSpeaker = GameObject.FindGameObjectWithTag("Player").GetComponent<SpeakingCharacter>();
            if (playerSpeaker == null)
            {
                Debug.LogError($@"Can't show options: the scene doesn't have an object tagged ""{playerSpeakerTag}"" with a {nameof(SpeakingCharacter)} component.");
                return;
            }
        }

        var speakerTransform = playerSpeaker.transform;
        var speakerBubbleOffset = playerSpeaker.BubbleOffset;
        var speakerTailTipOffset = playerSpeaker.TailTipPosition;

        string text = currentOptions[currentOptionIndex].Line.TextWithoutCharacterName.Text;
        speechBubbleController.ShowBubble(
            speakerTransform,
            text,
            speakerBubbleOffset,
            speakerTailTipOffset,
            optionCount: currentOptions.Count,
            this.currentOptionIndex);
        }

    public override void DismissLine (Action onDismissalComplete)
    {
        // Nothing to do; we will re-use the bubble if needed, if a line
        // comes in and uses the same speaker
        onDismissalComplete();
    }

    public override void NodeComplete (string nextNode, Action onComplete)
    {
        base.NodeComplete(nextNode, onComplete);
    }

    public override void OnLineStatusChanged (LocalizedLine dialogueLine)
    {
        Debug.Log($@"Line ""{dialogueLine.TextWithoutCharacterName.Text}"" changed state to {dialogueLine.Status}");
        base.OnLineStatusChanged(dialogueLine);
    }
}
