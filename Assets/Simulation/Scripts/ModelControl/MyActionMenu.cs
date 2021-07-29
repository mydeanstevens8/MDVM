namespace MDVM {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class MyActionMenu : MonoBehaviour
    {
        [Serializable]
        public struct ActionMenuAction
        {
            public string name;
            public UnityEvent action;
        }

        public class ButtonDelegateEvent : Button.ButtonClickedEvent
        {
            protected UnityEvent referenceEvent;

            protected MyActionMenu actionMenu;

            public ButtonDelegateEvent()
            {

            }

            public ButtonDelegateEvent(UnityEvent referenceEvent, MyActionMenu actionMenu)
            {
                this.referenceEvent = referenceEvent;
                this.actionMenu = actionMenu;
                AddListener(EventCall);
            }

            protected void EventCall()
            {
                referenceEvent.Invoke();
                
                if(actionMenu != null)
                {

                    actionMenu.CompleteActionMenu();
                }
            }
        }

        Canvas myCanvas = null;

        public GameObject debugImage = null;

        public GameObject uiToDisplay = null;

        private GameObject currentlyDisplaying = null;

        public GameObject actionMenuBaseTemplate = null;

        public GameObject actionMenuButtonTemplate = null;

        public GameObject deactivator = null;

        private List<ActionMenuAction> actionList = new List<ActionMenuAction>();

        // Settings for the action menu
        public float radialButtonDisplacementY = 350.0f;
        public float radialButtonDisplacementX = 250.0f;

        public AudioClip activationSound = null;
        public AudioClip deactivationSound = null;
        public AudioClip completeActSound = null;

        private AudioSource myAudioSource = null;

        private bool m_isActive = false;

        public bool isActive { get { return m_isActive; } }
        

        // Start is called before the first frame update
        void Start()
        {
            myCanvas = GetComponent<Canvas>();
            debugImage = GameObject.Find("CanvasDebugImage");

            myAudioSource = GetComponent<AudioSource>();

            RawDeactivateActionMenu();
        }

        // Update is called once per frame
        void Update()
        {
            if(m_isActive)
            {
                RotateActionMenuToTarget();
            }
        }

        protected void GenerateActionMenu(List<ActionMenuAction> actionList)
        {
            currentlyDisplaying = Instantiate(actionMenuBaseTemplate, transform);

            // Based on the length of the dictionary of actions, calculate the radial displacement value
            int dictLength = actionList.Count;

            int buttonPosition = 0;
            foreach(ActionMenuAction entry in actionList)
            {
                float radialDisplacementRaw = 2 * Mathf.PI * ((float)buttonPosition) / (dictLength);

                float radialDisplacementScale = 0.4f + 0.1f * dictLength;

                // Start from the top and head clockwise. In Unity, positive is up and negative is down for y.
                float horizontalRadialDisplacement = Mathf.Sin(radialDisplacementRaw) * radialButtonDisplacementX * radialDisplacementScale;
                float verticalRadialDisplacement = Mathf.Cos(radialDisplacementRaw) * radialButtonDisplacementY * radialDisplacementScale;

                float buttonAlignment = 
                    // Guard against floating point inaccuracies.
                    Mathf.Abs(horizontalRadialDisplacement) < 0.001f? 
                    0.5f :
                    0.5f - Mathf.Sign(horizontalRadialDisplacement) / 2;


                float buttonAlignmentVert = 0.5f + 0.5f * (Mathf.Sin(radialDisplacementRaw) >= 0 ?
                        Mathf.Pow(Mathf.Tan((radialDisplacementRaw / 2) - (Mathf.PI / 4)), 3.0f) :
                        -Mathf.Pow(Mathf.Tan((radialDisplacementRaw / 2) + (Mathf.PI / 4)), 3.0f));


                string description = entry.name;
                UnityEvent action = entry.action;

                GameObject newButton = Instantiate(actionMenuButtonTemplate, currentlyDisplaying.transform);

                // Null if it does not exist
                RectTransform rectTrans = newButton.GetComponent<RectTransform>();

                if(rectTrans != null)
                {
                    rectTrans.localPosition = new Vector3(horizontalRadialDisplacement, verticalRadialDisplacement, 0);
                    rectTrans.localRotation = Quaternion.identity;
                    rectTrans.localScale = Vector3.one;

                    rectTrans.pivot = new Vector2(buttonAlignment, buttonAlignmentVert);
                }

                // Set the text element
                TextMeshProUGUI textWidget = newButton.GetComponentInChildren<TextMeshProUGUI>();

                if(textWidget != null)
                {
                    textWidget.text = description;
                }

                // Set the button element
                Button buttonWidget = newButton.GetComponent<Button>();

                if(buttonWidget != null)
                {
                    ButtonDelegateEvent myEvent = new ButtonDelegateEvent(action, this);
                    
                    buttonWidget.onClick = myEvent;
                }

                // Set a delay for that button based on the position
                ButtonDelay delayAppearance = newButton.GetComponent<ButtonDelay>();

                if(delayAppearance != null)
                {
                    delayAppearance.popupDelay = 0.025f + 0.025f * buttonPosition;
                }


                // Next radial position
                buttonPosition++;
            }
        }

        public void AssignActions(List<ActionMenuAction> actions)
        {
            actionList = actions;
        }

        public void ActivateActionMenu(Vector3 worldPosition)
        {
            // Initially deactivate
            RawDeactivateActionMenu();

            if (activationSound != null && myAudioSource != null)
            {
                myAudioSource.PlayOneShot(activationSound);
            }

            if (uiToDisplay != null)
            {
                currentlyDisplaying = Instantiate(uiToDisplay, transform);
            }
            else
            {
                // Fills the currently displaying flag for us.
                GenerateActionMenu(actionList);
            }

            if (currentlyDisplaying.transform is RectTransform)
            {
                RectTransform rTrans = (RectTransform)currentlyDisplaying.transform;

                // Set to the center
                rTrans.pivot = new Vector2(0.5f, 0.5f);
                rTrans.anchorMin = new Vector2(0.5f, 0.5f);
                rTrans.anchorMax = new Vector2(0.5f, 0.5f);
                rTrans.localPosition = new Vector3(0, 0, 0);
                rTrans.localRotation = Quaternion.identity;
            }

            // Position ourselves at the activated position.
            transform.position = worldPosition;

            // Initial rotate to target. More rotates in the update function.
            RotateActionMenuToTarget();

            // Action menu debug
            if (debugImage != null)
            {
                debugImage.SetActive(true);
            }

            // Deactivator
            if (deactivator != null)
            {
                deactivator.SetActive(true);
            }

            m_isActive = true;
        }

        void RotateActionMenuToTarget()
        {
            Camera referenceCamera = myCanvas.worldCamera;

            transform.LookAt(referenceCamera.transform.position, Vector3.up);

            // Flip our canvas around.
            transform.localRotation = transform.localRotation * Quaternion.Euler(0, 180.0f, 0);
        }

        // Plays a sound on deactivation.
        public void DeactivateActionMenu()
        {
            if(deactivationSound != null && myAudioSource != null)
            {
                myAudioSource.PlayOneShot(deactivationSound);
            }
            RawDeactivateActionMenu();
        }

        public void CompleteActionMenu()
        {
            if (completeActSound != null && myAudioSource != null)
            {
                myAudioSource.PlayOneShot(completeActSound);
            }
            DeactivateActionMenu();
        }

        public void RawDeactivateActionMenu()
        {
            if (debugImage != null)
            {
                debugImage.SetActive(false);
            }

            if (currentlyDisplaying != null)
            {
                Destroy(currentlyDisplaying);
                currentlyDisplaying = null;
            }

            if (deactivator != null)
            {
                deactivator.SetActive(false);
            }

            m_isActive = false;
        }
    }
}

