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
            public ActionMenuAction[] subactions;
        }

        public class ButtonDelegateEvent : Button.ButtonClickedEvent
        {
            protected ActionMenuAction referenceEvent;

            protected MyActionMenu actionMenu;

            public ButtonDelegateEvent()
            {

            }

            public ButtonDelegateEvent(ActionMenuAction referenceEvent, MyActionMenu actionMenu)
            {
                this.referenceEvent = referenceEvent;
                this.actionMenu = actionMenu;
                AddListener(EventCall);
            }

            protected void EventCall()
            {
                if(actionMenu != null)
                {
                    if(referenceEvent.subactions.Length > 0)
                    {
                        Vector3 activatePosition = actionMenu.transform.position;
                        actionMenu.ActivateSubMenu(activatePosition, referenceEvent, new List<ActionMenuAction>(referenceEvent.subactions));
                    }
                    else
                    {
                        referenceEvent.action.Invoke();
                        actionMenu.CompleteActionMenu();
                    }
                }
            }
        }

        Canvas myCanvas = null;

        private GameObject currentlyDisplaying = null;

        public GameObject actionMenuBaseTemplate = null;

        public GameObject actionMenuButtonTemplate = null;

        public GameObject deactivator = null;

        private List<ActionMenuAction> actionList = new List<ActionMenuAction>();

        // Settings for the action menu
        public float radialButtonDisplacementY = 350.0f;
        public float radialButtonDisplacementX = 250.0f;

        public float buttonPopupDelayInterval = 0.025f;

        private AudioSource myAudioSource = null;

        private bool m_isActive = false;

        public bool isActive { get { return m_isActive; } }
        

        // Start is called before the first frame update
        void Start()
        {
            myCanvas = GetComponent<Canvas>();
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

        protected void GenerateActionMenu(ActionMenuAction? root, List<ActionMenuAction> actionList)
        {
            currentlyDisplaying = Instantiate(actionMenuBaseTemplate, transform);

            // Based on the length of the dictionary of actions, calculate the radial displacement value
            int dictLength = actionList.Count;

            float radialDisplacementScale = (dictLength < 2) ? 0 : (0.4f + 0.1f * dictLength);

            if (root != null)
            {
                ActionMenuAction foundRoot = (ActionMenuAction)root;

                radialDisplacementScale = Mathf.Max(radialDisplacementScale, 0.5f);

                string description = "<-  " + foundRoot.name;

                GameObject newButton = Instantiate(actionMenuButtonTemplate, currentlyDisplaying.transform);

                // Null if it does not exist
                RectTransform rectTrans = newButton.GetComponent<RectTransform>();

                if (rectTrans != null)
                {
                    rectTrans.localPosition = new Vector3(0, 0, 0);
                    rectTrans.localRotation = Quaternion.identity;
                    rectTrans.localScale = Vector3.one;

                    rectTrans.pivot = new Vector2(0.5f, 0.5f);
                }

                // Set the text element
                TextMeshProUGUI textWidget = newButton.GetComponentInChildren<TextMeshProUGUI>();

                if (textWidget != null)
                {
                    textWidget.text = description;
                }

                // Set the button element
                Button buttonWidget = newButton.GetComponent<Button>();

                if (buttonWidget != null)
                {
                    // ButtonDelegateEvent myEvent = new ButtonDelegateEvent(foundRoot, this);
                    // buttonWidget.onClick = myEvent;
                }
            }

            int buttonPosition = 0;
            foreach(ActionMenuAction entry in actionList)
            {
                float radialDisplacementRaw = 2 * Mathf.PI * ((float)buttonPosition) / dictLength;

                // Start from the top and head clockwise. In Unity, positive is up and negative is down for y.
                float horizontalRadialDisplacement = Mathf.Sin(radialDisplacementRaw) * radialButtonDisplacementX * radialDisplacementScale;
                float verticalRadialDisplacement = Mathf.Cos(radialDisplacementRaw) * radialButtonDisplacementY * radialDisplacementScale;

                float buttonAlignment = 
                    // Guard against floating point inaccuracies.
                    Mathf.Abs(horizontalRadialDisplacement) < 0.001f? 
                    0.5f :
                    0.5f - Mathf.Sign(horizontalRadialDisplacement) / 2;


                float buttonAlignmentVert = (dictLength < 2) ? 0.5f : 
                        0.5f + 0.5f * (Mathf.Sin(radialDisplacementRaw) >= 0 ?
                        Mathf.Pow(Mathf.Tan((radialDisplacementRaw / 2) - (Mathf.PI / 4)), 3.0f) :
                        -Mathf.Pow(Mathf.Tan((radialDisplacementRaw / 2) + (Mathf.PI / 4)), 3.0f));


                string description = entry.name;

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
                    ButtonDelegateEvent myEvent = new ButtonDelegateEvent(entry, this);
                    buttonWidget.onClick = myEvent;
                }

                // Set a delay for that button based on the position
                UI.ButtonDelay delayAppearance = newButton.GetComponent<UI.ButtonDelay>();

                if(delayAppearance != null)
                {
                    delayAppearance.popupDelay = buttonPopupDelayInterval + buttonPopupDelayInterval * buttonPosition;
                }


                // Next radial position
                buttonPosition++;
            }
        }

        public void AssignActions(List<ActionMenuAction> actions)
        {
            actionList = actions;
        }

        public void ActivateSubMenu(Vector3 worldPosition, ActionMenuAction? root, List<ActionMenuAction> actionList)
        {
            // Initially deactivate
            RawDeactivateActionMenu();

            // Fills the currently displaying flag for us.
            GenerateActionMenu(root, actionList);

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

            // Deactivator
            if (deactivator != null)
            {
                deactivator.SetActive(true);
            }

            m_isActive = true;
        }

        public void ActivateActionMenu(Vector3 worldPosition)
        {
            // Play sound
            UI.UISoundSystem.PlayS(UI.UISoundSystem.Get().actionMenuActivate);

            ActivateSubMenu(worldPosition, null, actionList);
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
            UI.UISoundSystem.PlayS(UI.UISoundSystem.Get().actionMenuDeactivate);
            RawDeactivateActionMenu();
        }

        public void CompleteActionMenu()
        {
            UI.UISoundSystem.PlayS(UI.UISoundSystem.Get().actionMenuComplete);
            DeactivateActionMenu();
        }

        public void RawDeactivateActionMenu()
        {
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

