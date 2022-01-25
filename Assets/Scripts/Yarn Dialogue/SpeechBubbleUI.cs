using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

[Serializable]
public struct TailPoints {
    public RectTransform tailBase;
    public RectTransform midUpper;
    public RectTransform midLower;
    public RectTransform tip;
}

// Manages speech bubble UI
public class SpeechBubbleUI : MonoBehaviour
{
    //// Base UI Variables
    [SerializeField]
    private Vector2 minBubbleSize = new Vector2(100,40);

    [Header("Bubble Components")]
    public Image background;
    public LineRenderer tail;
    // public RectTransform[] tailPoints = new RectTransform[4];
    public TailPoints tailPoints;

    private RectTransform bubbleRect;
    private RectTransform tailRect;
    private Sequence tweenSequence;
    private AnimationCurve tailCurve;
    private Keyframe[] tailKeys;

    private RectTransform RectTransform => GetComponent<RectTransform>();


    //// Choice Variables

    [Header("Choice Components")]
    public GameObject leftArrow;
    public GameObject rightArrow;
    [Tooltip("Parent object of all pips")]
    public RectTransform pipContainer;
    // List of all available pips
    // (Image type to reduce GetComponent calls)
    [Tooltip("List of all available pips.\n\n(If empty or incorrect, drag new pips in or set size 0 and drag all pips in)")]
    public List<Image> pips = new List<Image>();

    [Header("Pip Colors")]
    [Tooltip("Color for pip corresponding to currently selected option. Will be applied to pip automatically")]
    public Color activeColor;
    [Tooltip("Color for pip(s) corresponding to all un-selected available options. Will be applied to pip automatically")]
    public Color inactiveColor;

    private Image[] currentPips; // List of active pips available for UI manip (Image type to reduce GetComponent calls)
    private int activePip;
    private float pipWidth;
    // Distance between pips
    private float pipDistance;

    public TextMeshProUGUI TextMesh => GetComponentInChildren<TextMeshProUGUI>();

    private void Awake() {
        bubbleRect = background.GetComponent<RectTransform>();
        tailRect = tail.GetComponent<RectTransform>();

        // Get canvas components
        
        tweenSequence = DOTween.Sequence();

        // Duplicate material to allow unique values for this instance
        background.material = new Material(background.material);
        tail.material = new Material(tail.material);

        activePip = -1;
        DisableChoiceUI();
        tail.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        // Destroy material instance that won't be GC'd
        DestroyImmediate(background.material);
        DestroyImmediate(tail.material);
    }

    // Sets up necessary UI positions
    public void SetPositions(Vector3 bubblePosition, Vector3 tailPosition){
        RectTransform.anchoredPosition = bubblePosition;
        PositionTail(tailPosition);
    }


    //// Visuals

    // Performs opening-specific animations w/o tail
    public void TweenOpen(Vector2 toSize, float time, int fps, Ease openEase, UnityEvent onOpenComplete){
        // Reset tween sequence
        tweenSequence = DOTween.Sequence(); 

        TweenSize(toSize, minBubbleSize, time, fps, openEase, onOpenComplete);
    }

    // Performs opening-specific animations
    // TODO: use tail size param
    public void TweenOpen(Vector2 toSize, float bubbleTime, float tailTime, int fps, Ease openEase, UnityEvent onOpenComplete){
        // Hide bubble while tail animates
        bubbleRect.gameObject.SetActive(false);

        // Make sure tail is hidden first frame
        SetTailValues(percentVisible: 0);

        // Reset tween sequence
        tweenSequence = DOTween.Sequence();

        // Add tail open tween to sequence
        float vis = 0;
        tweenSequence.Append(
        // TODO: add ease/fps
            DOTween.To(() => vis, x => vis = x, 1, tailTime)
                .OnStart(() => DisableChoiceUI())
                .OnUpdate(() => SetTailValues(percentVisible: vis))
            );

        // Continue tween
        TweenSize(toSize, minBubbleSize, bubbleTime, fps, openEase, onOpenComplete);
    }

    // Performs closing-specific animations
    public void TweenClose(float bubbleTime, float tailTime, int fps, Ease closeEase, UnityEvent onCloseComplete, UnityEvent onTailComplete = null){

        // If we're in the middle of a typewriter effect, stop it now
        if (currentTypewriterEffect != null) {
            StopCoroutine(currentTypewriterEffect);
            currentTypewriterEffect = null;
        }

        // Removes choice UI before bubble closes
        // onTailComplete += DisableChoiceUI;

        // Set up tail animation
        tailCurve = new AnimationCurve();
        tailKeys = tail.widthCurve.keys;

        // Add tail disappear tween to sequence
        float thicc = tail.widthCurve.keys[1].time;
        tweenSequence.Append(
            // TODO: add ease/fps
            DOTween.To(() => thicc, x => thicc = x, 0, tailTime)
            .OnUpdate(() => SetTailValues(length: thicc))
            .OnComplete(() => onTailComplete?.Invoke())
        );

        // Continue tween
        TweenSize(minBubbleSize, bubbleTime, fps, closeEase, onCloseComplete, onTailComplete);
    }

    // Tweens bubble size, 'from' -> 'to'
    private void TweenSize(Vector2 to, Vector2 from, float time, int fps, Ease ease, UnityEvent onTweenComplete){
        bubbleRect.sizeDelta = from;

        TweenSize(to, time, fps, ease, onTweenComplete);
    }

    // Tweens bubble size, current size -> 'to'
    public void TweenSize(Vector2 to, float time, int fps, Ease ease, UnityEvent onTweenComplete, UnityEvent onTailComplete = null){
        // TODO?: use size-diff modifier to make tween more visually consistent? (cover similar size per timeframe)
        // float speedMod = Mathf.Abs(to.sqrMagnitude / bubbleRect.sizeDelta.sqrMagnitude) * tweenSizeDiffMod;
        // ??????????
        // Debug.Log(speedMod);

        // Tween bubble size
        tweenSequence.Append(
            bubbleRect.DOSizeDelta(to, time)
            .SetEase(EaseFactory.StopMotion(fps, ease))
            .OnStart(() => {if(currentPips?.Length > 0) EnableChoiceUI(currentPips.Length);})
            .OnUpdate(ResizeVisuals)
            .OnComplete(onTweenComplete.Invoke)
        );
        // TODO?: set tween complete delegate call at apex of out-back ease, if possible (how does this work w/ big->small tweens??)
    }

    // Updates tail shader
    public void SetTailValues(float? percentVisible = null, float? length = null){
        if(percentVisible != null)
            tail.material.SetFloat("PercentVisible", percentVisible.Value);
        
        if(length != null){
            tailKeys[1].time = length.Value;
            // tailKeys[0].value = tail.widthCurve.keys[0].value - tail.widthCurve.keys[0].value * length.Value; // Changes total width as well
            tailCurve.keys = tailKeys;
            tail.widthCurve = tailCurve;
        }
    }

    // Handles resizing bubble visuals relative to tail and container
    public void ResizeVisuals(){
        // Make sure bubble is active
        if(!bubbleRect.gameObject.activeSelf)
            bubbleRect.gameObject.SetActive(true);

        // Offset bubble position so bottom doesn't move relative to tail
        bubbleRect.localPosition = new Vector3(
            bubbleRect.localPosition.x,
            bubbleRect.sizeDelta.y / 2 - RectTransform.sizeDelta.y / 2 ,
            bubbleRect.localPosition.z
        );

        SetBubbleValues();
    }

    // Updates bubble shader
    public void SetBubbleValues(){
        // Debug.Log(bubbleBG.materialForRendering);
        // bubbleBG.materialForRendering.SetFloat("_SpeedX", parameters.placeholder);
        // Debug.Log(parameters.placeholder);
    }

    // Adjusts values for new tail position based on prefab position
    public void PositionTail(Vector3 tipPosition){
        // TODO: Make this more flexible and look better- use relative distance from base again?
        // Get old and new tip positions
        Vector3 oldTipPos = tailPoints.tip.localPosition;
        tailPoints.tip.position = RectTransform.TransformPoint(tipPosition);
        Vector3 newTipPos = tailPoints.tip.localPosition;

        // Get other point positions
        Vector3 basePos = tailPoints.tailBase.localPosition;
        Vector3 upPos = tailPoints.midUpper.localPosition;
        Vector3 lowPos = tailPoints.midLower.localPosition;

        // Set middle point positions as an offset of (old/new tip->base offset ratio * old tip->midpts offset) from  position
        tailPoints.midUpper.localPosition = newTipPos - new Vector3(
            ((basePos.x - newTipPos.x) / (basePos.x - oldTipPos.x)) * (oldTipPos.x - upPos.x),
            ((basePos.y - newTipPos.y) / (basePos.y - oldTipPos.y)) * (oldTipPos.y - upPos.y)
        );
        tailPoints.midLower.localPosition = newTipPos - new Vector3(
            ((basePos.x - newTipPos.x) / (basePos.x - oldTipPos.x)) * (oldTipPos.x - lowPos.x),
            ((basePos.y - newTipPos.y) / (basePos.y - oldTipPos.y)) * (oldTipPos.y - lowPos.y)
        );

        // Set tail active and rendering
        tail.gameObject.SetActive(true);
    }


    //// Choice Functions

    // Sets up UI for given number of pips
    public void InitializeChoiceUI(int pipNum){
        currentPips = new Image[pipNum];

        // Setup pip list (if needed)
        if(pips.Count <= 0)
            pips = new List<Image>(pipContainer.GetComponentsInChildren<Image>());

        int diff = pips.Count - pipNum;

        // Qty of pips greater than available pips
        // (Should never happen unless something is set up wrong, went wrong, or the writer went wild and didn't tell anyone)
        if(diff < 0){
            Debug.LogError(string.Format("Oops all pips! Copy-paste {0} more to the speech bubble prefab lol", pipNum - pips.Count));
            diff = 0;
            pipNum = pips.Count;
        }

        // Set up current pips
        for(int i = 0; i < pips.Count; i++){
            // Within range of needed num of pips
            if(i < pipNum){
                // pips[i].gameObject.SetActive(true);
                currentPips[i] = pips[i];
                // Make sure inactive color is correct
                pips[i].color = inactiveColor;
            }
            else
                pips[i].gameObject.SetActive(false);
        }

        // Make sure first pip is set active
        // (Assumes first choice is always displayed to user first)
        activePip = 0;

        // Offset container to re-center the number of pips
        // (Math works based on being 5 wide and 10 apart. if diff spacing needed, subtract width from spacing (pip X-pos diff))
        pipWidth = pips[0].GetComponent<RectTransform>().rect.width;
        pipContainer.anchoredPosition = new Vector2(pipWidth * diff, pipContainer.anchoredPosition.y); 
    }

    // Sets 'index' pip active and makes sure previously active pip set inactive
    public void ActivatePip(int index){
        pips[index].color = activeColor;
        // Set previous pip inactive
        if(activePip != index && activePip != -1)
            pips[activePip].color = inactiveColor;
        activePip = index;
    }

    // Enables current choice UI
    public void EnableChoiceUI(int optionCount)
    {
        if (currentPips == null || currentPips.Length != optionCount)
        {
            // We need a different number of pips!
            InitializeChoiceUI(optionCount);
        }

        // Enable arrows
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);

        foreach (Image pip in currentPips)
            pip.gameObject.SetActive(true);

        ActivatePip(activePip);
    }

    // Disables all choice UI
    public void DisableChoiceUI(){
        // Disable arrows
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);

        // Disable all available pips
        if(pips.Count > 0)
            foreach(Image pip in pips)
                pip.gameObject.SetActive(false);
    }

    private Coroutine currentTypewriterEffect;

    public void ShowTypewriterEffect(string text, int charactersPerSecond, System.Action onComplete = null) {
        if (currentTypewriterEffect != null) {
            StopCoroutine(currentTypewriterEffect);
        }

        TextMesh.text = text;

        currentTypewriterEffect = StartCoroutine(DoShowTypewriterEffect(charactersPerSecond, onComplete));
    }

    private IEnumerator DoShowTypewriterEffect(int charactersPerSecond, System.Action onComplete)
    {
        // Start with everything invisible
        TextMesh.maxVisibleCharacters = 0;

        // Wait a single frame to let the text component process its
        // content, otherwise text.textInfo.characterCount won't be
        // accurate
        yield return null;

        // How many visible characters are present in the text?
        var characterCount = TextMesh.textInfo.characterCount;

        // Early out if letter speed is zero or text length is zero
        if (charactersPerSecond <= 0 || characterCount == 0)
        {
            TextMesh.maxVisibleCharacters = characterCount;
            currentTypewriterEffect = null;
            if (onComplete != null)
            {
                onComplete.Invoke();
            }
            yield break;
        }

        // Convert 'letters per second' into its inverse
        float secondsPerLetter = 1.0f / charactersPerSecond;

        // If lettersPerSecond is larger than the average framerate, we
        // need to show more than one letter per frame, so simply adding 1
        // letter every secondsPerLetter won't be good enough (we'd cap out
        // at 1 letter per frame, which could be slower than the user
        // requested.)
        //
        // Instead, we'll accumulate time every frame, and display as many
        // letters in that frame as we need to in order to achieve the
        // requested speed.
        var accumulator = Time.deltaTime;

        while (TextMesh.maxVisibleCharacters < characterCount)
        {

            // We need to show as many letters as we have accumulated time
            // for.
            while (accumulator >= secondsPerLetter)
            {
                TextMesh.maxVisibleCharacters += 1;
                accumulator -= secondsPerLetter;
            }
            accumulator += Time.deltaTime;

            yield return null;

        }
        TextMesh.maxVisibleCharacters = characterCount;

        currentTypewriterEffect = null;

        // Wrap up by invoking our completion handler.
        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }
}