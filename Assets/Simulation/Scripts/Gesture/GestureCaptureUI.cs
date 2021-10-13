/* * *
Copyright (c) 2021 Dean Stevens and affiliates.

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files ("MDVM"), to deal in 
MDVM without restriction, including without limitation the rights to use, 
copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
MDVM, and to permit persons to whom MDVM is furnished to do so, subject to 
the following conditions:

The above copyright notice and this permission notice shall be included in 
all copies or substantial portions of MDVM.

MDVM IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR 
A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL DEAN STEVENS, 
OTHER AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH MDVM OR THE USE OR OTHER DEALINGS IN MDVM.

If you reside in Australia, the above disclaimer does not affect any consumer 
guarantees automatically given to you under the Competition and Consumer Act 
2010 (Commonwealth) and the Australian Consumer Law.
 * * */

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MDVM.UI;

namespace MDVM.Gesture
{
    public class GestureCaptureUI : MonoBehaviour
    {
        public OVRHand LeftHand = null;
        public OVRHand RightHand = null;

        public GestureDataGatherer dataGather = null;

        public Material gestureCaptureSelectionMaterial = null;
        Material activeSelMat = null;

        public Color gestureCaptureModeActivatedColor = Color.green;
        public Color gestureCaptureModeDectivatedColor = Color.white;

        // Gesture capture mode enabled
        bool gestureCaptureModeEnabled = false;

        // Check was pinching
        bool wasPinchingHand = false;

        public OVRHand.HandFinger gestureCaptureFingerPinchCode = OVRHand.HandFinger.Ring;
        public KeyCode gestureCaptureDebugHotkey = KeyCode.M;

        public AudioClip gestureCaptured = null;
        private void Start()
        {
            // Assign a material if we have a mesh renderer
            MeshRenderer render = GetComponent<MeshRenderer>();

            // Only assign when not null
            if(gestureCaptureSelectionMaterial)
            {
                render.material = gestureCaptureSelectionMaterial;
                activeSelMat = render.material;

                activeSelMat.color = gestureCaptureModeDectivatedColor;
            }

            if(dataGather != null)
            {
                LeftHand = dataGather.handL;
                RightHand = dataGather.handR;
            }
        }

        private void Update()
        {
            // To update whenever we get a hand press.

            // Ensure both hand refs are not null
            if(LeftHand && RightHand)
            {
                // Capture data on the left hand if we get a ring finger pinch on the right hand.
                bool leftHandPinch = LeftHand.GetFingerIsPinching(gestureCaptureFingerPinchCode);
                bool rightHandPinch = RightHand.GetFingerIsPinching(gestureCaptureFingerPinchCode);

                if(leftHandPinch || rightHandPinch)
                {
                    // On pinch and on gesture capture mode
                    if(gestureCaptureModeEnabled && (!wasPinchingHand))
                    {
                        // One, but not both, XOR
                        if(rightHandPinch ^ leftHandPinch)
                        {
                            OnGestureHandPinch(rightHandPinch);
                        }
                    }

                    wasPinchingHand = true;
                }
                else
                {
                    wasPinchingHand = false;
                }

                // Debug hotkey activate for left hand - capturing right hand data.
                if(Input.GetKeyUp(gestureCaptureDebugHotkey) && gestureCaptureModeEnabled)
                {
                    OnGestureHandPinch(false);
                }
            }

            // When gesture mode is off, disable gesture capture mode
            if(GestureMode.Instance.MyMode != GestureMode.Mode.Gesture && gestureCaptureModeEnabled)
            {
                ToggleGestureCaptureMode(false);
            }
        }

        protected void OnGestureHandPinch(bool rightHandPinched)
        {
            // Capture hand data of the other hand.
            if(rightHandPinched)
            {
                CaptureHandDataOn(LeftHand);
            }
            else
            {
                CaptureHandDataOn(RightHand);
            }
        }

        protected void CaptureHandDataOn(OVRHand hand)
        {
            if(dataGather)
            {
                try
                {
                    HandData handData = dataGather.CaptureHandData(hand);

                    // Hand data to be printed out onto the console.
                    // Saving it is for later
                    GestureDataSerializer.Instance.PrintJSONRepresentation(handData);

                    // Show a dialog for hand capture
                    GameObject sublayer = new GameObject();

                    TextMeshProUGUI tmPro = sublayer.AddComponent<TextMeshProUGUI>();

                    tmPro.text = GestureDataSerializer.Instance.GetJSONRepresentation(handData);
                    tmPro.color = Color.black;
                    tmPro.fontSize = 36;
                    tmPro.alignment = TextAlignmentOptions.TopLeft;

                    ModalDisplay.Instance.ShowModal(sublayer, "Hand Data Captured", false);
                    Destroy(sublayer);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());

                    // Show a dialog for hand capture error
                    GameObject sublayer = new GameObject();

                    TextMeshProUGUI tmPro = sublayer.AddComponent<TextMeshProUGUI>();

                    tmPro.text = e.ToString();
                    tmPro.color = Color.black;
                    tmPro.fontSize = 36;
                    tmPro.alignment = TextAlignmentOptions.TopLeft;

                    ModalDisplay.Instance.ShowModal(sublayer, "Hand capture debug info", false);
                    Destroy(sublayer);
                }
                
            }
        }

        public void ToggleGestureCaptureMode(bool enabled)
        {
            gestureCaptureModeEnabled = enabled;

            GestureMode gMode = GestureMode.Instance;

            if(enabled && (gMode.MyMode == GestureMode.Mode.Select))
            {
                // Set to gesture mode. The user can change this back later.
                gMode.MyMode = GestureMode.Mode.Gesture;
            }

            if(gestureCaptureSelectionMaterial)
            {
                activeSelMat.color = (enabled) ? gestureCaptureModeActivatedColor : gestureCaptureModeDectivatedColor;
            }
        }

        public bool IsGestureCaptureModeEnabled()
        {
            return gestureCaptureModeEnabled;
        }

        public void ToggleGestureCaptureModeUI()
        {
            ToggleGestureCaptureMode(!IsGestureCaptureModeEnabled());
        }
    }
}
