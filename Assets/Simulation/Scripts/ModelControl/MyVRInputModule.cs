using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MDVM 
{
    public class MyVRInputModule : OVRInputModule
    {
        [Header("Mode")]
        public bool leftHandMode = false;

        [Header("Controllers")]

        [Tooltip("The pinch mask for the left controller to launch click events. Acts as an addition to the joy pad click button.")]
        public OVRInput.Button leftControllerMask = OVRInput.Button.None;
        [Tooltip("The pinch mask for the right controller to launch click events. Acts as an addition to the joy pad click button.")]
        public OVRInput.Button rightControllerMask = OVRInput.Button.None;

        [Header("Hands")]
        [Tooltip("The left hand for event purposes.")]
        public OVRHand leftHand = null;

        [Tooltip("The right hand for event purposes.")]
        public OVRHand rightHand = null;

        [Tooltip("The pinch mask for the left hand to launch click events.")]
        public OVRHand.HandFinger leftHandPinchMask = OVRHand.HandFinger.Index;
        [Tooltip("The pinch mask for the right hand to launch click events.")]
        public OVRHand.HandFinger rightHandPinchMask = OVRHand.HandFinger.Index;

        [Tooltip("Whether to turn on hand detection or not.")]
        public bool enableHandDetection = true;

        [Header("Flags")]
        [Tooltip("Whether to enable the input module for inputting.")]
        public bool inputEnabled = true;

        protected bool wasHandPinching = false;

        // Overrides from OVR Input module to allow for hand presses.
        /// <summary>
        /// Get state of button corresponding to gaze pointer
        /// </summary>
        /// <returns></returns>
        override protected PointerEventData.FramePressState GetGazeButtonState()
        {
            if(!inputEnabled)
            {
                return PointerEventData.FramePressState.NotChanged;
            }

            var pressed = Input.GetKeyDown(gazeClickKey) || OVRInput.GetDown(joyPadClickButton);
            var released = Input.GetKeyUp(gazeClickKey) || OVRInput.GetUp(joyPadClickButton);

#if UNITY_ANDROID && !UNITY_EDITOR
            pressed |= Input.GetMouseButtonDown(0);
            released |= Input.GetMouseButtonUp(0);
#endif

            // Extra controller buttons
            bool extraDown = leftHandMode ? OVRInput.GetDown(leftControllerMask) : OVRInput.GetDown(rightControllerMask);
            bool extraUp = leftHandMode ? OVRInput.GetUp(leftControllerMask) : OVRInput.GetUp(rightControllerMask);

            // Hand detection
            if(enableHandDetection)
            {
                bool leftHandPinching = leftHand ? leftHand.GetFingerIsPinching(leftHandPinchMask) : false;
                bool rightHandPinching = rightHand ? rightHand.GetFingerIsPinching(rightHandPinchMask) : false;

                bool isHandPinching = leftHandMode ? leftHandPinching : rightHandPinching;

                pressed |= extraDown | (isHandPinching && (!wasHandPinching));
                released |= extraUp | (wasHandPinching && (!isHandPinching));

                wasHandPinching = isHandPinching;
            }

            // End hand detection and extra controller buttons

            if (pressed && released)
                return PointerEventData.FramePressState.PressedAndReleased;
            if (pressed)
                return PointerEventData.FramePressState.Pressed;
            if (released)
                return PointerEventData.FramePressState.Released;
            return PointerEventData.FramePressState.NotChanged;

        }

        /// <summary>
        /// A copy from PointerInputModule but used for enter and exit events even with a locked cursor.
        /// Process movement for the current frame with the given pointer event.
        /// </summary>
        protected override void ProcessMove(PointerEventData pointerEvent)
        {
            if (!inputEnabled)
            {
                return;
            }

            var targetGO = pointerEvent.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointerEvent, targetGO);
        }
    }
}

