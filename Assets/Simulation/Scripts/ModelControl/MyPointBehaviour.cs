namespace MDVM
{
    using Oculus;
    using OculusSampleFramework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class MyPointBehaviour : MonoBehaviour
    {
        [Serializable]
        protected class PointRaycastEvent : UnityEvent<PointRaycastData> { }

        [Serializable]
        protected struct PointRaycastData
        {
            public PointRaycastData(Vector3 position, Vector3 direction, bool isLeftHand)
            {
                Position = position;
                Direction = direction;
                IsLeftHand = isLeftHand;
            }

            public Vector3 Position { get; private set; }
            public Vector3 Direction { get; private set; }
            public bool IsLeftHand { get; private set; }
        }

        // On which hand are we on?
        public bool LeftHand = true;

        public GameObject HandReference = null;
        public GameObject ControllerReference = null;
        public GameObject HandEyeAnchor = null;

        public bool HideOnHand = false;
        public bool HideOnSystemGesture = true;
        public bool UnityEditorDebugMode = true;
        public bool PointerEnabled = true;
        public bool HideInGestureMode = true;

        protected bool isPointerEnabledHere = true;

        public Vector3 ControllerOffset = new Vector3();
        public Vector3 ControllerRotation = new Vector3();

        public Vector3 HandOffset = new Vector3();
        public Vector3 HandRotation = new Vector3();

        public Vector3 HandRotationOffset = new Vector3();

        public bool AutoCalculateHandTransform = true;

        // Long press time in seconds
        public float LongPressTime = 0.3f;

        bool CurrentlyHandsMode = false;

        PointRaycastEvent OnPress = new PointRaycastEvent();
        PointRaycastEvent OnPressStart = new PointRaycastEvent();
        PointRaycastEvent OnPressEnd = new PointRaycastEvent();

        PointRaycastEvent OnLongPressStart = new PointRaycastEvent();
        PointRaycastEvent OnLongPressEnd = new PointRaycastEvent();
        PointRaycastEvent OnLongPress = new PointRaycastEvent();

        bool IsPressing = false;
        float PressTimeStart = 0.0f;
        bool LongPressingSet = false;

        public Color PressedColor = new Color(0, 1, 0, 0.75f);
        public Color UnpressedColor = new Color(1, 1, 1, 0.25f);
        public Color InactiveColor = new Color(1, 1, 1, 0.0f);

        public OVRHand.HandFinger HandButtonPinchMask = OVRHand.HandFinger.Index;
        public OVRInput.Button ControllerButtonPinchMask = OVRInput.Button.Any;

        public AudioSource PointAudioSource = null;

        public AudioClip PinchSoundClip = null;
        public AudioClip LongPinchSoundClip = null;

        public bool IsCurrentlyPressing
        {
            get
            {
                return IsPressing;
            }
        }

        Vector3 CurrentHandPosition = new Vector3();
        Vector3 CurrentHandDirection = new Vector3();

        bool isEditorDebug = false;

        // Start is called before the first frame update
        void Start()
        {
            OnPressStart.AddListener(PointMaterialChangeOnPress);
            OnPressStart.AddListener(PointSoundOnPinch);
            OnPressStart.AddListener(SwitchPointersOnPress);

            OnPressEnd.AddListener(PointMaterialChangeOnPress);

            OnLongPressStart.AddListener(PointSoundOnLongPinch);

            PointMaterialChangeOnPress();

            isEditorDebug = UnityEditorDebugMode && Application.isEditor;
        }

        protected void CheckEnableFlags()
        {
            OVRHand MyHand = HandReference != null ? HandReference.GetComponent<OVRHand>() : null;

            if (!PointerEnabled)
            {
                isPointerEnabledHere = false;
            }
            else if(CurrentlyHandsMode && HideOnHand)
            {
                isPointerEnabledHere = false;
            }
            else if(CurrentlyHandsMode && HideOnSystemGesture && MyHand.IsSystemGestureInProgress)
            {
                isPointerEnabledHere = false;
            }
            else
            {
                Gesture.GestureMode mode = Gesture.GestureMode.Instance;
                if (HideInGestureMode && mode != null && !mode.IsSelectMode())
                {
                    isPointerEnabledHere = false;
                }
                else
                {
                    isPointerEnabledHere = true;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            CheckEnableFlags();
            PointerPositionUpdate();
            PointEventCall();
        }

        protected void PointerPositionUpdate()
        {
            // Update the position of the pointing object based on such information.
            OVRHand MyHand = HandReference != null ? HandReference.GetComponent<OVRHand>() : null;

            // Ensure non-null.
            CurrentlyHandsMode = (MyHand != null) && (MyHand.IsTracked);

            MeshRenderer mr = GetComponentInChildren<MeshRenderer>();

            if (mr != null)
            {
                if(Application.isEditor)
                {
                    mr.enabled = false;
                }
                else
                {
                    mr.enabled = isPointerEnabledHere;
                }
            }

            Vector3 Position = ControllerOffset;
            Vector3 Rotation = ControllerRotation;

            if (CurrentlyHandsMode)
            {
                Position = HandOffset;
                Rotation = HandRotation;
            }

            Quaternion RotationQuat = Quaternion.Euler(Rotation);

            if (CurrentlyHandsMode && AutoCalculateHandTransform)
            {
                Vector3 ExtraPosition = Position;
                Quaternion ExtraRotationQuat = RotationQuat;

                // Should exist
                OVRSkeleton MySkeleton = MyHand.GetComponent<OVRSkeleton>();

                // Use these bones to rotate our model.
                OVRBone ThumbEnd = null;
                OVRBone ThumbMiddle = null;

                OVRBone ThumbStart = null;

                OVRBone IndexEnd = null;
                OVRBone IndexMiddle = null;

                OVRBone IndexStart = null;

                foreach (OVRBone MyBone in MySkeleton.Bones)
                {
                    switch (MyBone.Id)
                    {
                        case OVRSkeleton.BoneId.Hand_ThumbTip:
                            ThumbEnd = MyBone;
                            break;
                        case OVRSkeleton.BoneId.Hand_Thumb3:
                            ThumbMiddle = MyBone;
                            break;
                        case OVRSkeleton.BoneId.Hand_Thumb0:
                            ThumbStart = MyBone;
                            break;

                        case OVRSkeleton.BoneId.Hand_IndexTip:
                            IndexEnd = MyBone;
                            break;
                        case OVRSkeleton.BoneId.Hand_Index3:
                            IndexMiddle = MyBone;
                            break;
                        case OVRSkeleton.BoneId.Hand_Index1:
                            IndexStart = MyBone;
                            break;
                    }
                }

                GameObject CameraCentre = HandEyeAnchor != null ? HandEyeAnchor : GameObject.Find("CenterEyeAnchor");
                Vector3 CameraPosition = CameraCentre.transform.position;

                Vector3 MidwayPosition = (ThumbEnd.Transform.position + IndexStart.Transform.position) / 2;
                Vector3 MidwayVector = MidwayPosition - CameraPosition;

                ExtraPosition = MidwayPosition;

                ExtraRotationQuat = Quaternion.LookRotation(MidwayVector, Vector3.up).normalized;
                ExtraRotationQuat *= Quaternion.Euler(HandRotationOffset);

                transform.position = ExtraPosition;
                transform.rotation = ExtraRotationQuat;
            }
            else
            {
                transform.localPosition = Position;
                transform.localRotation = RotationQuat;
            }

            CurrentHandPosition = transform.position;
            CurrentHandDirection = transform.forward;
        }

        protected void PointEventCall()
        {
            OVRHand MyHand = HandReference != null ? HandReference.GetComponent<OVRHand>() : null;
            PointRaycastData myPointRaycast = new PointRaycastData(CurrentHandPosition, CurrentHandDirection, LeftHand);

            bool eventConditions = MyHand.GetFingerIsPinching(HandButtonPinchMask);

            eventConditions |= OVRInput.Get(ControllerButtonPinchMask);

            eventConditions |= (isEditorDebug && Input.GetMouseButton(0));

            // Disable events if we are not enabled.
            if(!isPointerEnabledHere)
            {
                eventConditions = false;
            }

            float CurrentPressTime = Time.time;

            bool LongPressing = IsPressing && (CurrentPressTime - PressTimeStart >= LongPressTime);

            if (eventConditions)
            {
                // Beginning to press and while pressing
                if (!IsPressing)
                {
                    // Beginning to press
                    IsPressing = true;
                    PressTimeStart = CurrentPressTime;
                    OnPressStart.Invoke(myPointRaycast);
                }
                else
                {
                    // While pressing
                    if(LongPressing && !LongPressingSet)
                    {
                        OnLongPressStart.Invoke(myPointRaycast);
                        LongPressingSet = true;
                    }
                }
            }
            else
            {
                // When the pressing ends.
                if (IsPressing)
                {
                    IsPressing = false;
                    LongPressingSet = false;
                    PressTimeStart = 0.0f;
                    OnPressEnd.Invoke(myPointRaycast);

                    // Different behaviour if long pressing. (Note that this is passed by value)
                    if(LongPressing)
                    {
                        OnLongPressEnd.Invoke(myPointRaycast);
                        OnLongPress.Invoke(myPointRaycast);
                    }
                    else
                    {
                        OnPress.Invoke(myPointRaycast);
                    }
                }
            }
        }

        protected void PointMaterialChangeOnPress(PointRaycastData param = new PointRaycastData())
        {
            MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
            Color myColor = IsCurrentlyPressing ? PressedColor : UnpressedColor;
            mr.material.color = myColor;
        }


        protected void PointSoundOnPinch(PointRaycastData param)
        {
            if (PointAudioSource != null)
            {
                if (PinchSoundClip != null)
                {
                    PointAudioSource.PlayOneShot(PinchSoundClip);
                }
            }
        }

        protected void PointSoundOnLongPinch(PointRaycastData param)
        {
            if (PointAudioSource != null)
            {
                if (LongPinchSoundClip != null)
                {
                    PointAudioSource.PlayOneShot(LongPinchSoundClip);
                }
            }
        }

        protected void SwitchPointersOnPress(PointRaycastData param)
        {
            // Debug.Log("Pointer switched on press. ", this);
            UI.SwitchPointerControl switcher = UI.SwitchPointerControl.Get();

            switcher.SwitchPointer(gameObject);
        }
    }

}
