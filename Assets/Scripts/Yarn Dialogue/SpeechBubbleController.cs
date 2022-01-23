using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the presentation of speech bubbles and the text they contain.
/// </summary>
public class SpeechBubbleController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    [SerializeField] private RectTransform container;

    [SerializeField] private SpeechBubbleUI speechBubblePrefab;

    public SpeechBubbleParameters SpeechBubbleParameters { get; set; }

    public Vector2 defaultBubbleOffset = Vector2.zero;

    /// <summary>
    /// The current speech bubble that's visible to the player.
    /// </summary>
    private SpeechBubbleUI currentBubble;

    /// <summary>
    /// The Transform of the game object that the current speech bubble is
    /// being presented from.
    /// </summary>
    private Transform currentSpeaker;

    /// <summary>
    /// The bubble offset of the most recently displayed bubble.
    /// </summary>
    private Vector2 currentBubbleOffset;

    /// <summary>
    /// The tail tip offset of the most recently displayed bubble.
    /// </summary>
    private Vector2 currentTailTipOffset;

    /// <summary>
    /// Gets a value indicating whether this <see
    /// cref="SpeechBubbleController"/> is currently displaying a
    /// collection of options or not.
    /// </summary>
    public bool IsRunningOptions { get; private set; }

    public void ClearOptions()
    {
        if (currentBubble != null)
        {
            currentBubble.DisableChoiceUI();
        }
    }

    public void CloseBubble(System.Action onClosed = null)
    {
        currentBubble.TextMesh.text = "";

        currentBubble.TweenClose(SpeechBubbleParameters.bubbleCloseTime, SpeechBubbleParameters.tailCloseTime, SpeechBubbleParameters.tweenFPS, SpeechBubbleParameters.closeEase, () =>
        {
            if (currentBubble != null)
            {
                Destroy(currentBubble.gameObject);
            }

            currentBubble = null;
            currentSpeaker = null;

            if (onClosed != null)
                {
                onClosed.Invoke();
                }
        });
    }

    /// <summary>
    /// Determines appropriate bubble size for <paramref name="line"/>.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="bubbleText"></param>
    /// <param name="maxWidth"></param>
    /// <returns>A bubble size that will contain 'line'</returns>
    private static Vector2 GetLineSizeAndBreaks(string line, TextMeshProUGUI bubbleText, float maxWidth)
    {
        var bubbleTextRect = bubbleText.GetComponent<RectTransform>();

        string currentLine = line;

        // Find text container values
        var totalTextPadding = bubbleTextRect.offsetMin + new Vector2(Mathf.Abs(bubbleTextRect.offsetMax.x), Mathf.Abs(bubbleTextRect.offsetMax.y));
        var maxTextWidth = maxWidth - totalTextPadding.x;

        var linebreaks = new List<int>();

        // Variable setup for linebreak determination
        if (linebreaks.Count > 0)
            linebreaks.Clear();
        string segment = ""; // Current segment of line
        string nextSegment = ""; // Current segment plus next word
        int prevSpace = 0;  // Index of previous space
        int segmentWords = 0; // Number of words in segment (Determines words longer than max length)

        var preferredVals = new Vector2();
        var totalLineSize = new Vector2();

        // TODO: Store cache for options' text linebreaks to prevent doing
        // this every time user navigates btwn options

        // Determine positions for linebreaks in current line to maintain
        // maximum width
        for (int i = 0; i <= currentLine.Length - 1; i++)
        {
            // nextSegment = segment + currentLine[i]; 
            nextSegment += currentLine[i];

            // Word-end has been reached
            if (currentLine[i] == ' ')
            {
                // TODO: Consolidate breakable char definition(s)
                segmentWords++;

                // Max width has been reached
                preferredVals = bubbleText.GetPreferredValues(nextSegment.Trim());
                if (preferredVals.x > maxTextWidth)
                {

                    // Use length of segment up to the word that exceeds
                    // max width
                    if (segmentWords == 1)
                        Debug.LogWarning("Line contains word longer than maximum allowed textbox width");
                    else
                        preferredVals = bubbleText.GetPreferredValues(segment.Trim());

                    // Add linebreak and incorporate line size to rolling
                    // total
                    linebreaks.Add(prevSpace);
                    totalLineSize = new Vector2(Mathf.Max(totalLineSize.x, preferredVals.x), totalLineSize.y + preferredVals.y);

                    // Reset segment values to last word before max length
                    // was reached
                    i = prevSpace;
                    segment = "";
                    nextSegment = "";
                    segmentWords = 0;
                    prevSpace = 0;
                }
                else
                {
                    segment = nextSegment;
                    prevSpace = i;
                }
            }
        }

        // Incorporate size of last line in total calculations
        if (nextSegment != "")
        {
            preferredVals = bubbleText.GetPreferredValues(nextSegment.Trim());
            totalLineSize = new Vector2(Mathf.Max(totalLineSize.x, preferredVals.x), totalLineSize.y + preferredVals.y);
        }

        Vector2 finalSize = totalLineSize + totalTextPadding;

        return finalSize;
    }

    /// <summary>
    /// Displays a speech bubble with the given <paramref name="text"/>, at
    /// the screen position of <paramref name="speaker"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method animates the speech bubble open, and then displays the
    /// text using a typewriter-like effect.
    /// </para>
    /// <para>
    /// If this <see cref="SpeechBubbleController"/> is currently
    /// presenting a speech bubble, and <paramref name="speaker"/> is the
    /// same as the previous invocation of ShowBubble, the existing speech
    /// bubble will be re-used. Otherwise, the existing speech bubble (if
    /// present) will be closed, and a new speech bubble will be opened.
    /// </para>
    /// </remarks>
    /// <param name="speaker">The <see cref="Transform"/> that the speech
    /// bubble should be attached to.</param>
    /// <param name="text">The text to display in the speech
    /// bubble.</param>
    /// <param name="bubbleOffset">The distance, in the local space of <see
    /// cref="canvas"/>, that the speech bubble should be placed, relative
    /// to the screen position of <paramref name="speaker"/>.</param>
    /// <param name="tailTipOffset">The distance, in the local space of
    /// <see cref="canvas"/>, that the tip of the speech bubble's tail
    /// should be placed, relative to the screen position of <paramref
    /// name="speaker"/>.</param>
    /// <param name="optionCount">The number of option indicators to show.
    /// If zero or lower, no option indicators are shown.</param>
    /// <param name="selectedOption">The zero-based index of the option
    /// indicator to highlight. This value must be less than <paramref
    /// name="optionCount"/>.</param>
    /// <param name="onComplete">A method to call when the bubble and its
    /// has appeared on screen.</param>
    public void ShowBubble(Transform speaker, string text, Vector2 bubbleOffset, Vector2 tailTipOffset, int optionCount = 0, int selectedOption = 0, System.Action onComplete = null)
    {
        if (currentBubble != null && currentSpeaker != speaker)
        {
            // A bubble is active, and it's from a different speaker than
            // what we want to show. Close it, and then create a new one.
            CloseBubble(() =>
            {
                ShowBubble(speaker, text, bubbleOffset, tailTipOffset, optionCount, selectedOption, onComplete);
            });
            return;
        }

        // If we're using the same speaker as last time, or if we don't
        // have a bubble at all, we can re-use the bubble. This means we
        // won't instantiate a new one, and instead of using TweenOpen,
        // we'll use TweenSize.
        bool bubbleIsNew = speaker != currentSpeaker || currentBubble == null;

        if (bubbleIsNew)
        {
            currentBubble = Instantiate(speechBubblePrefab, container);
            currentBubble.TextMesh.fontSize = SpeechBubbleParameters.fontSize;
        }

        currentSpeaker = speaker;

        if (bubbleOffset == Vector2.zero) {
            bubbleOffset = defaultBubbleOffset;
        }

        GetPositions(speaker, bubbleOffset, tailTipOffset, out Vector2 bubblePosition, out Vector2 tailTipPosition);

        currentBubble.SetPositions(bubblePosition, tailTipPosition);

        currentBubble.TextMesh.text = "";

        

        currentBubbleOffset = bubbleOffset;
        currentTailTipOffset = tailTipOffset;

        var size = GetLineSizeAndBreaks(text, currentBubble.TextMesh, SpeechBubbleParameters.maximumWidth);

        if (bubbleIsNew)
        {
            if (SpeechBubbleParameters.hasTail)
            {
                currentBubble.TweenOpen(size, SpeechBubbleParameters.bubbleOpenTime, SpeechBubbleParameters.tailOpenTime, SpeechBubbleParameters.tweenFPS, SpeechBubbleParameters.openEase, OnOpenComplete);
            }
            else
            {
                currentBubble.TweenOpen(size, SpeechBubbleParameters.bubbleOpenTime, SpeechBubbleParameters.tweenFPS, SpeechBubbleParameters.openEase, OnOpenComplete);
            }
        }
        else
        {
            // We may still have choices from an earlier presentation, so
            // clear them here if needed
            if (optionCount < 1) {
                currentBubble.DisableChoiceUI();
            }

            currentBubble.TweenSize(size, SpeechBubbleParameters.baseTweenTime, SpeechBubbleParameters.tweenFPS, SpeechBubbleParameters.baseTweenEase, OnOpenComplete);
        }

        void OnOpenComplete()
        {
            // After a bubble has finished opening or changing size,
            // display the options UI if needed, and begin the typewriter
            // effect.

            if (optionCount < 1)
            {
                currentBubble.DisableChoiceUI();
            }
            else
            {
                currentBubble.EnableChoiceUI(optionCount);
                currentBubble.ActivatePip(selectedOption);
            }

            currentBubble.ShowTypewriterEffect(text, SpeechBubbleParameters.charactersPerSecond, onComplete);
        }

    }

    private void GetPositions(Transform speaker, Vector2 bubbleOffset, Vector2 tailTipOffset, out Vector2 bubblePosition, out Vector2 tailTipPosition)
    {
        Vector3 position;

        if (speaker == null)
        {
            position = Vector3.zero;
        }
        else
        {
            position = speaker.transform.position;
        }

        var speakerScreenPosition = Camera.main.WorldToScreenPoint(position);

        Camera camera;

        switch (canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                // Use 'null' when dealing with overlay canvases
                camera = null;
                break;
            case RenderMode.ScreenSpaceCamera:
                camera = canvas.worldCamera;
                break;
            default:
                camera = Camera.main;
                break;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, speakerScreenPosition, camera, out bubblePosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, speakerScreenPosition, camera, out tailTipPosition);

        bubblePosition += bubbleOffset;
        tailTipPosition += tailTipOffset;
    }
}
