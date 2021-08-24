using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM.Model
{
    public class AxisControls : MDVMControl
    {
        public MDVMPlot.AxisDirection direction = MDVMPlot.AxisDirection.Z;
        public float axisOffset = 0.1f;

        public GameObject axisScaleHandle = null;
        public Vector3 axisScaleHandleOffset = Vector3.zero;

        public GameObject axisFilterMaxHandle = null;
        public Vector3 axisFilterMaxHandleOffset = Vector3.zero;


        public GameObject axisFilterMinHandle = null;
        public Vector3 axisFilterMinHandleOffset = Vector3.zero;


        public GameObject axisExtensor = null;
        public Vector3 axisExtensorOffset = Vector3.zero;


        public GameObject axisBackBuffer = null;
        public Vector3 axisBackBufferOffset = Vector3.zero;

        private bool HasStarted = false;

        // Start is called before the first frame update
        public override void OnControlStart()
        {
            HasStarted = true;
            Refresh();
        }

        // Update is called once per frame
        // Don't call this on the control provided functions as this is necessary for updating the actual graph.
        void Update()
        {
            if(plotReference != null)
            {
                UpdateAxisComponents();
            }
        }

        protected bool IsCurrentAxisEnabled()
        {
            return plotReference.HasAxis(direction);
        }

        protected float GetCurrentAxisLength()
        {
            Vector3 currentDimensions = plotReference.GetVisualisationDimensions();

            switch (direction)
            {
                case MDVMPlot.AxisDirection.X:
                    return currentDimensions.x;
                case MDVMPlot.AxisDirection.Y:
                    return currentDimensions.y;
                case MDVMPlot.AxisDirection.Z:
                    return currentDimensions.z;
                default:
                    return 1;
            }

        }

        protected Vector2 GetCurrentAxisFilter()
        {
            return plotReference.GetFilter(direction);
        }

        protected void SetCurrentAxisFilter(float min, float max)
        {
            plotReference.SetFilter(direction, new Vector2(min, max));
        }


        protected void SetCurrentAxisMinFilter(float min)
        {
            plotReference.SetFilter(direction, new Vector2(min, GetCurrentAxisFilter().y));
        }

        protected void SetCurrentAxisMaxFilter(float max)
        {
            plotReference.SetFilter(direction, new Vector2(GetCurrentAxisFilter().x, max));
        }

        protected void SetCurrentAxisLength(float newLength)
        {
            Vector3 cd = plotReference.GetVisualisationDimensions();

            switch (direction)
            {
                case MDVMPlot.AxisDirection.X:
                    plotReference.SetVisualisationDimensions(new Vector3(newLength, cd.y, cd.z));
                    break;
                case MDVMPlot.AxisDirection.Y:
                    plotReference.SetVisualisationDimensions(new Vector3(cd.x, newLength, cd.z));
                    break;
                case MDVMPlot.AxisDirection.Z:
                    plotReference.SetVisualisationDimensions(new Vector3(cd.x, cd.y, newLength));
                    break;
                default:
                    break;
            }
        }

        protected bool IsBeingGrabbed(GameObject comp)
        {
            MyGrabbable grabbable = comp.GetComponent<MyGrabbable>();

            if (grabbable == null) grabbable = comp.GetComponentInChildren<MyGrabbable>();

            return grabbable != null && grabbable.BeingGrabbed;
        }

        protected void UpdateExtensor()
        {
            // Stop all updates while an object is being grabbed by a grabber.
            if (axisExtensor != null)
            {
                if(IsBeingGrabbed(axisExtensor))
                {
                    // Listen to
                    float calcAxisLength = axisExtensor.transform.localScale.z;
                    SetCurrentAxisLength(calcAxisLength);
                }
                else
                {
                    // Update
                    float axisLength = GetCurrentAxisLength();

                    Vector3 oldSc = axisExtensor.transform.localScale;
                    axisExtensor.transform.localScale = new Vector3(oldSc.x, oldSc.y, axisLength / 2);

                    axisExtensor.transform.localPosition = axisExtensorOffset + new Vector3(0, 0, axisLength / 2);
                }
            }
        }

        protected void UpdateScaleHandle()
        {
            if (axisScaleHandle != null)
            {
                if (IsBeingGrabbed(axisScaleHandle))
                {
                    // Listen to
                    float calcAxisLength = axisScaleHandle.transform.localPosition.z - axisScaleHandleOffset.z;
                    SetCurrentAxisLength(calcAxisLength);
                }
                else
                {
                    // Update
                    float axisLength = GetCurrentAxisLength();
                    axisScaleHandle.transform.localPosition = axisScaleHandleOffset + new Vector3(0, 0, axisLength);
                }
            }
        }

        protected void UpdateBackBuffer()
        {
            if (axisBackBuffer != null && !IsBeingGrabbed(axisBackBuffer))
            {
                axisBackBuffer.transform.localPosition = axisBackBufferOffset;
            }
        }

        protected void UpdateFilterMinHandle()
        {
            if (axisFilterMinHandle != null)
            {
                float axisLength = GetCurrentAxisLength();

                if (IsBeingGrabbed(axisFilterMinHandle))
                {
                    // Listen to
                    float calcAxisFilter = (axisFilterMinHandle.transform.localPosition.z / axisLength) - axisFilterMinHandleOffset.z;
                    SetCurrentAxisMinFilter(calcAxisFilter);
                }
                else
                {
                    // Update
                    float minFilter = GetCurrentAxisFilter().x;
                    float maxFilter = GetCurrentAxisFilter().y;

                    axisFilterMinHandle.transform.localPosition = axisFilterMinHandleOffset + new Vector3(0, 0, minFilter * axisLength);

                    // Allow the player to grab it according to the range.
                    MyGrabbable aFGrab = axisFilterMinHandle.GetComponent<MyGrabbable>();
                    if(aFGrab != null)
                    {
                        MyGrabbable.GrabLockSettings agl = aFGrab.lockPosition;
                        agl.maxZ = maxFilter * axisLength;
                        agl.minZ = 0;

                        aFGrab.lockPosition = agl;
                    }
                }
            }
        }

        protected void UpdateFilterMaxHandle()
        {
            if (axisFilterMaxHandle != null)
            {
                float axisLength = GetCurrentAxisLength();

                if (IsBeingGrabbed(axisFilterMaxHandle))
                {
                    // Listen to
                    float calcAxisFilter = (axisFilterMaxHandle.transform.localPosition.z / axisLength) - axisFilterMaxHandleOffset.z;
                    SetCurrentAxisMaxFilter(calcAxisFilter);
                }
                else
                {
                    // Update
                    float minFilter = GetCurrentAxisFilter().x;
                    float maxFilter = GetCurrentAxisFilter().y;
                    axisFilterMaxHandle.transform.localPosition = axisFilterMaxHandleOffset + new Vector3(0, 0, maxFilter * axisLength);

                    // Allow the player to grab it according to the range.
                    MyGrabbable aFGrab = axisFilterMaxHandle.GetComponent<MyGrabbable>();
                    if (aFGrab != null)
                    {
                        MyGrabbable.GrabLockSettings agl = aFGrab.lockPosition;
                        agl.maxZ = axisLength;
                        agl.minZ = minFilter * axisLength;

                        aFGrab.lockPosition = agl;
                    }
                }
            }
        }

        protected void UpdateAxisComponents()
        {
            if(IsCurrentAxisEnabled())
            {
                SetAxisComponentsEnabled(true);
                UpdateExtensor();
                UpdateScaleHandle();
                UpdateBackBuffer();

                UpdateFilterMinHandle();
                UpdateFilterMaxHandle();
            }
            else
            {
                SetAxisComponentsEnabled(false);
            }
        }

        protected void SetAxisComponentsEnabled(bool enabled)
        {
            if (axisScaleHandle != null) axisScaleHandle.SetActive(enabled);
            if (axisFilterMinHandle != null) axisFilterMinHandle.SetActive(enabled);
            if (axisFilterMaxHandle != null) axisFilterMaxHandle.SetActive(enabled);
            if (axisExtensor != null) axisExtensor.SetActive(enabled);
            if (axisBackBuffer != null) axisBackBuffer.SetActive(enabled);
        }

        public void Refresh()
        {
            if (Application.isPlaying && !HasStarted)
            {
                // Wait until we have started in play mode.
                return;
            }

            SetMDVMPlot(GetComponentInParent<MDVMPlot>());
            if(plotReference != null)
            {
                UpdateAxisComponents();
            }

            PositionAxis();
        }

        private void OnValidate()
        {
            Refresh();
        }

        public void PositionAxis()
        {
            if(plotReference != null)
            {
                bool hasX = plotReference.HasAxis(MDVMPlot.AxisDirection.X);
                bool hasY = plotReference.HasAxis(MDVMPlot.AxisDirection.Y);
                bool hasZ = plotReference.HasAxis(MDVMPlot.AxisDirection.Z);

                bool has3 = hasX && hasY && hasZ;

                Vector3 m01Offset = new Vector3(-axisOffset, -axisOffset, -axisOffset);
                Vector3 mRotationEuler = new Vector3();

                switch(direction)
                {
                    case MDVMPlot.AxisDirection.X:
                        m01Offset.x = 0.0f;
                        mRotationEuler.y = 90.0f;

                        if (!has3 && hasY)
                        {
                            m01Offset.z = 0.0f;
                        }
                        if (!has3 && hasZ)
                        {
                            m01Offset.y = 0.0f;
                        }
                        break;
                    case MDVMPlot.AxisDirection.Y:
                        m01Offset.y = 0.0f;
                        mRotationEuler.x = -90.0f;

                        if (!has3 && hasX)
                        {
                            m01Offset.z = 0.0f;
                        }
                        if (!has3 && hasZ)
                        {
                            m01Offset.x = 0.0f;
                        }
                        break;
                    case MDVMPlot.AxisDirection.Z:
                        m01Offset.z = 0.0f;

                        if (!has3 && hasX)
                        {
                            m01Offset.y = 0.0f;
                        }
                        if (!has3 && hasY)
                        {
                            m01Offset.x = 0.0f;
                        }
                        break;
                }

                transform.localPosition = m01Offset;
                transform.localRotation = Quaternion.Euler(mRotationEuler);
            }
        }
    }
}

