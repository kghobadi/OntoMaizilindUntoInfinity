using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Yarn.Unity
{
    public class MyOptionListView : DialogueViewBase
    {
        [SerializeField] CanvasGroup canvasGroup;

        [SerializeField] MyOptionView optionViewPrefab;

        [SerializeField] TextMeshProUGUI lastLineText;

        [SerializeField] float fadeTime = 0.1f;

        [SerializeField] bool showUnavailableOptions = false;

        public bool canSelectOption;
        
        [Header("Controls for Options UI")]
        public Sprite optionOneControl;
        public Sprite optionTwoControl;
        
        public Sprite optionOneController;
        public Sprite optionTwoController;
        
        // A cached pool of OptionView objects so that we can reuse them
        List<MyOptionView> optionViews = new List<MyOptionView>();

        // The method we should call when an option has been selected.
        Action<int> OnOptionSelected;

        // The line we saw most recently.
        LocalizedLine lastSeenLine;

        public void Start()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void Reset()
        {
            canvasGroup = GetComponentInParent<CanvasGroup>();
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            // Don't do anything with this line except note it and
            // immediately indicate that we're finished with it. RunOptions
            // will use it to display the text of the previous line.
            lastSeenLine = dialogueLine;
            onDialogueLineFinished();
        }

        public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
        {
            // Hide all existing option views
            foreach (var optionView in optionViews)
            {
                optionView.gameObject.SetActive(false);
            }

            canSelectOption = true;

            // If we don't already have enough option views, create more
            while (dialogueOptions.Length > optionViews.Count)
            {
                var optionView = CreateNewOptionView();
                optionView.SetupOption(optionViews.Count - 1, this);
                optionView.gameObject.SetActive(false);
            }

            // Set up all of the option views
            for (int i = 0; i < dialogueOptions.Length; i++)
            {
                SetOptionView(i);
            }

            // Update the last line, if one is configured
            if (lastLineText != null)
            {
                if (lastSeenLine != null) {
                    lastLineText.gameObject.SetActive(true);
                    lastLineText.text = lastSeenLine.Text.Text;
                } else {
                    lastLineText.gameObject.SetActive(false);
                }
            }

            // Note the delegate to call when an option is selected
            OnOptionSelected = onOptionSelected;

            // Fade it all in
            StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeTime));

            /// <summary>
            /// Creates and configures a new <see cref="OptionView"/>, and adds
            /// it to <see cref="optionViews"/>.
            /// </summary>
            MyOptionView CreateNewOptionView()
            {
                var optionView = Instantiate(optionViewPrefab);
                optionView.transform.SetParent(transform, false);
                optionView.transform.SetAsLastSibling();

                optionView.OnOptionSelected = OptionViewWasSelected;
                optionViews.Add(optionView);

                return optionView;
            }

            void SetOptionView(int index)
            {
                var optionView = optionViews[index];
                var option = dialogueOptions[index];

                if (option.IsAvailable == false && showUnavailableOptions == false)
                {
                    // Don't show this option.
                    return;
                }

                optionView.gameObject.SetActive(true);

                optionView.Option = option;

                // The first available option is selected by default
                if (index == 0)
                {
                    optionView.Select();
                }
            }

            /// <summary>
            /// Called by <see cref="OptionView"/> objects.
            /// </summary>
            void OptionViewWasSelected(DialogueOption option)
            {
                StartCoroutine(Effects.FadeAlpha(canvasGroup, 1, 0, fadeTime, () => OnOptionSelected(option.DialogueOptionID)));
            }
        }
    }
}
