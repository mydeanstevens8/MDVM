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

using UnityEngine;

namespace MDVM.Gesture
{
    class GestureMode : MonoBehaviour
    {
        private static GestureMode myInstance = null;

        public static GestureMode Instance
        {
            get
            {
                return myInstance;
            }
            private set
            {
                myInstance = value;
            }
        }

        [System.Serializable]
        public enum Mode
        {
            Select,
            Gesture
        }

        // Enable mode switching
        public bool enableModeSwitching = true;

        protected Mode mode = Mode.Select;

        // Pinch to switch to gesture mode.
        public OVRHand.HandFinger modeSwitchMaskHand = OVRHand.HandFinger.Pinky;

        // Command key to switch to gesture mode
        public OVRInput.Button modeSwitchMaskController = OVRInput.Button.Two;

        // Keyboard key to switch to gesture mode
        public KeyCode modeSwitchMaskKey = KeyCode.G;

        // Sound effects
        public AudioClip gestureModeOnSound = null;
        public AudioClip selectModeOnSound = null;

        private MyVRInputModule vrInput = null;

        private bool onStarted = false;

        private bool wereHandsPinching = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if(vrInput == null)
            {
                vrInput = GetComponent<MyVRInputModule>();

                // Enforce a property update here.
                MyMode = MyMode;
            }

            onStarted = true;
        }

        private void Update()
        {
            if(enableModeSwitching)
            {
                CheckModeSwitch();
            }
        }

        protected void CheckModeSwitch()
        {
            bool modeSwitchConditions = OVRInput.GetUp(modeSwitchMaskController);
            modeSwitchConditions |= Input.GetKeyUp(modeSwitchMaskKey);

            // Both hands to be activated.
            bool areHandsPinching = (
                vrInput.leftHand.GetFingerIsPinching(modeSwitchMaskHand) &&
                vrInput.rightHand.GetFingerIsPinching(modeSwitchMaskHand)
                );

            if(areHandsPinching && !wereHandsPinching)
            {
                modeSwitchConditions = true;
            }

            wereHandsPinching = areHandsPinching;

            if (modeSwitchConditions)
            {
                if(MyMode == Mode.Gesture)
                {
                    MyMode = Mode.Select;
                }
                else if(MyMode == Mode.Select)
                {
                    MyMode = Mode.Gesture;
                }
            }
        }

        public Mode MyMode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;

                vrInput.inputEnabled = mode == Mode.Select;

                if(onStarted)
                {
                    switch (mode)
                    {
                        case Mode.Select:
                            UI.UISoundSystem.PlayS(selectModeOnSound);
                            break;
                        case Mode.Gesture:
                            UI.UISoundSystem.PlayS(gestureModeOnSound);
                            break;
                    }
                }
            }
        }

        public bool IsSelectMode()
        {
            return MyMode == Mode.Select;
        }

        public bool IsGestureMode()
        {
            return MyMode == Mode.Gesture;
        }
    }
}
