using System;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using InControl;

namespace Yarn.Unity
{
    public class MyOptionView : UnityEngine.UI.Selectable, ISubmitHandler, IPointerClickHandler, IPointerEnterHandler
    {
        private MyOptionListView optionListView;
        
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] bool showCharacterName = false;
        [SerializeField] private Image controlImage;
        
        public Action<DialogueOption> OnOptionSelected;

        DialogueOption _option;

        bool hasSubmittedOptionSelection = false;

        public int optionIndex = -1;
        private InputDevice inputDevice;
        private InputDeviceClass lastDeviceClass = InputDeviceClass.Unknown;
        
        public DialogueOption Option
        {
            get => _option;

            set
            {
                _option = value;
                
                hasSubmittedOptionSelection = false;

                // When we're given an Option, use its text and update our
                // interactibility.
                if (showCharacterName)
                {
                    text.text = value.Line.Text.Text;
                }
                else
                {
                    text.text = value.Line.TextWithoutCharacterName.Text;
                }
                interactable = value.IsAvailable;
            }
        }
        
        private void Update()
        {
            //get input device 
            inputDevice = InputManager.ActiveDevice;
            
            CheckForInputs();

            //if last device class does not match current & our option index has been set. 
            if (lastDeviceClass != inputDevice.DeviceClass && optionIndex >= 0)
            {
                SetControlImage();
                //update last device class
                lastDeviceClass = inputDevice.DeviceClass;
            }
        }
        
        /// <summary>
        /// Checks for active inputs. 
        /// </summary>
        void CheckForInputs()
        {
            //take no more input!
            if (hasSubmittedOptionSelection || !optionListView.canSelectOption)
            {
                return;
            }
            
            //left click/ square button for Dialogue Option 1
            if (Input.GetMouseButtonDown(0) || inputDevice.Action3.WasPressed)
            {
                CheckChoiceOne();
            }
            //right click/ circle button for Dialogue Option 2
            if (Input.GetMouseButtonDown(1) || inputDevice.Action2.WasPressed)
            {
                CheckChoiceTwo();
            }
        }

        void CheckChoiceOne()
        {
            if (optionIndex == 0)
            {
                InvokeOptionSelected();
            }
        }
        
        void CheckChoiceTwo()
        {
            if (optionIndex == 1)
            {
                InvokeOptionSelected();
            }
        }

        /// <summary>
        /// Tells us which option it is and set the control sprite. 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="control"></param>
        public void SetupOption(int index, MyOptionListView optionList)
        {
            optionIndex = index;
            optionListView = optionList;
            hasSubmittedOptionSelection = false;
            SetControlImage();
        }

        /// <summary>
        /// Designates correct Control image sprite in the UI. 
        /// </summary>
        void SetControlImage()
        {
            //get input device 
            inputDevice = InputManager.ActiveDevice;
            
            //set control image as first option 
            if (optionIndex == 0)
            {
                //controller
                if (inputDevice.DeviceClass == InputDeviceClass.Controller)
                {
                    controlImage.sprite = optionListView.optionOneController;
                }
                //mk
                else
                {
                    controlImage.sprite = optionListView.optionOneControl;
                }
            }
            //set control image as second option 
            else if (optionIndex == 1)
            {
                //controller
                if (inputDevice.DeviceClass == InputDeviceClass.Controller)
                {
                    controlImage.sprite = optionListView.optionTwoController;
                }
                //mk
                else
                {
                    controlImage.sprite = optionListView.optionTwoControl;
                }
            }
        }

        // If we receive a submit or click event, invoke our "we just selected
        // this option" handler.
        public void OnSubmit(BaseEventData eventData)
        {
            InvokeOptionSelected();
        }

        public void InvokeOptionSelected()
        {
            // We only want to invoke this once, because it's an error to
            // submit an option when the Dialogue Runner isn't expecting it. To
            // prevent this, we'll only invoke this if the flag hasn't been cleared already.
            if (hasSubmittedOptionSelection == false)
            {
                OnOptionSelected.Invoke(Option);
                hasSubmittedOptionSelection = true;
                optionListView.canSelectOption = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            InvokeOptionSelected();
        }

        // If we mouse-over, we're telling the UI system that this element is
        // the currently 'selected' (i.e. focused) element. 
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.Select();
        }
    }
}
