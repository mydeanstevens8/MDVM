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
    public class PointRaycastEvent : UnityEvent <PointRaycastData> { }

    [Serializable]
    public struct PointRaycastData
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

    public Vector3 ControllerOffset = new Vector3();
    public Vector3 ControllerRotation = new Vector3();

    public Vector3 HandOffset = new Vector3();
    public Vector3 HandRotation = new Vector3();

    public Vector3 HandRotationOffset = new Vector3();

    public bool AutoCalculateHandTransform = true;

    bool CurrentlyHandsMode = false;

    public PointRaycastEvent OnPress = new PointRaycastEvent();
    public PointRaycastEvent OnPressStart = new PointRaycastEvent();
    public PointRaycastEvent OnPressEnd = new PointRaycastEvent();

    bool IsPressing = false;

    public Color PressedColor = new Color(0, 1, 0, 0.75f);
    public Color UnpressedColor = new Color(1, 1, 1, 0.25f);

    public bool IsCurrentlyPressing
    {
        get
        {
            return IsPressing;
        }
    }

    Vector3 CurrentHandPosition = new Vector3();
    Vector3 CurrentHandDirection = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        OnPressStart.AddListener(PointMaterialChangeOnPress);
        OnPressEnd.AddListener(PointMaterialChangeOnPress);

        PointMaterialChangeOnPress();
    }

    // Update is called once per frame
    void Update()
    {
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

        if (HideOnHand)
        {
            if (mr != null)
            {
                // Disable when hands are on, enable when hands are not on.
                mr.enabled = !CurrentlyHandsMode;
            }
        }
        else if (HideOnSystemGesture)
        {
            if(mr != null && CurrentlyHandsMode)
            {
                // Disable on a system gesture.
                mr.enabled = !MyHand.IsSystemGestureInProgress;
            }
        }
        else
        {
            if (mr != null)
            {
                mr.enabled = true;
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

        GameObject ExtraRot = transform.Find("ExtraRot").gameObject;

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
                        ThumbStart = MyBone;
                        break;
                }
            }

            GameObject CameraCentre = HandEyeAnchor != null? HandEyeAnchor : GameObject.Find("CenterEyeAnchor");
            Vector3 CameraPosition = CameraCentre.transform.position;

            Vector3 MidwayPosition = (ThumbEnd.Transform.position + IndexEnd.Transform.position) / 2;
            Vector3 MidwayVector = MidwayPosition - CameraPosition;

            ExtraPosition = MidwayPosition;

            ExtraRotationQuat = Quaternion.FromToRotation(Vector3.forward, MidwayVector).normalized;
            ExtraRotationQuat *= Quaternion.Euler(HandRotationOffset);

            transform.position = ExtraPosition;
            transform.rotation = ExtraRotationQuat;

            ExtraRot.transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.localPosition = Position;
            transform.localRotation = RotationQuat;

            ExtraRot.transform.localRotation = Quaternion.identity;
        }

        CurrentHandPosition = transform.position;
        CurrentHandDirection = transform.forward;
    }

    protected void PointEventCall()
    {
        OVRHand MyHand = HandReference != null ? HandReference.GetComponent<OVRHand>() : null;
        PointRaycastData myPointRaycast = new PointRaycastData(CurrentHandPosition, CurrentHandDirection, LeftHand);

        bool eventConditions = MyHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

        eventConditions |= OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);

        if (eventConditions)
        {
            if(!IsPressing)
            {
                IsPressing = true;
                OnPressStart.Invoke(myPointRaycast);
            }
        }
        else
        {
            if (IsPressing)
            {
                IsPressing = false;
                OnPressEnd.Invoke(myPointRaycast);
                OnPress.Invoke(myPointRaycast);
            }
        }
    }

    protected void PointMaterialChangeOnPress(PointRaycastData param = new PointRaycastData())
    {
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        Color myColor = IsCurrentlyPressing ? PressedColor : UnpressedColor;
        mr.material.color = myColor;
    }

    protected void PointInitiateGrabber(PointRaycastData param)
    {
        OVRGrabber OurGrabber = GetComponentInParent<OVRGrabber>();

    }
}
