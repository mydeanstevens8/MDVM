using System;
using System.Collections.Generic;

using UnityEngine;
using MDVM.Model;
using MDVM.UI;
using System.Collections;

namespace MDVM.Gesture
{
    class RangeGesture2D : Gesture, IGestureEndable
    {
        bool LeftHandInPosition = false;
        bool RightHandInPosition = false;

        bool GestureWaitingInPosition = false;
        bool GestureInPosition = false;

        // Plane for cutting off at the ends of the hands.
        Plane LHandPlane;
        Plane RHandPlane;

        // Plane for outward projection of selection.
        Plane OutSelPlane;

        // Circle visualizations for range selection.
        public GameObject RangeSelVis = null;

        protected GameObject lRSelVis = null;
        protected GameObject rRSelVis = null;

        public AudioClip OnActivateClip = null;
        public AudioClip OnCompleteClip = null;
        public AudioClip OnDeselectClip = null;

        float rangeGestureWaitDelay = 0.7f;

        void Start()
        {
            GestureInit();

            if(RangeSelVis)
            {
                lRSelVis = Instantiate(RangeSelVis, transform);
                rRSelVis = Instantiate(RangeSelVis, transform);

                lRSelVis.SetActive(false);
                rRSelVis.SetActive(false);
            }
        }

        void Update()
        {
            PerformCheck();

            // Force the ending of the gesture if we are not in gesture mode but our gesture is active.
            if(GestureInPosition && (!GestureMode.Instance.IsGestureMode()))
            {
                OnGestureForceEnd();
            }

            // Match gestures and check if they are in position.
            if(LeftHandInPosition && RightHandInPosition
                && (!GestureWaitingInPosition) && (!GestureInPosition))
            {
                GestureWaitingInPosition = true;
                StartCoroutine(CheckInPositionCoroutine());
            }

            if(GestureInPosition)
            {
                UpdateInPosition();
            }
        }

        protected IEnumerator CheckInPositionCoroutine()
        {
            yield return new WaitForSeconds(rangeGestureWaitDelay);

            if (LeftHandInPosition && RightHandInPosition && (!GestureInPosition))
            {
                GestureInPosition = true;
                OnGestureInPosition();
            }

            GestureWaitingInPosition = false;
            yield return new WaitForEndOfFrame();
        }


        protected override void OnHandGesture(OVRHand hand)
        {
            // On the specific hand gesture activated.
            // Activate on start, given the hand.
            if (hand == LeftHand)
            {
                LeftHandInPosition = true;
            }

            if (hand == RightHand)
            {
                RightHandInPosition = true;
            }
        }

        protected override void OnHandGestureEnd(OVRHand hand)
        {
            // On the specific hand gesture activated.
            // Deactivate on end, given the hand.
            if (hand == LeftHand)
            {
                LeftHandInPosition = false;
            }

            if (hand == RightHand)
            {
                RightHandInPosition = false;
            }
        }

        // Keep the gesture in position until the gesture is complete.
        protected void OnGestureInPosition()
        {
            // Our formula is to devise a way to draw a line between our two hands, then convert them into normal planes on two sides.

            // Call our first update here to fill in data.
            UpdateInPosition();

            if(lRSelVis && rRSelVis)
            {
                lRSelVis.SetActive(true);
                rRSelVis.SetActive(true);
            }

            UISoundSystem.PlayS(OnActivateClip);
        }

        protected void UpdateInPosition()
        {
            // Recalculate our planes from the positions of our hands.
            Vector3 lhp = LeftHand.transform.position;
            Vector3 rhp = RightHand.transform.position;

            Vector3 displr = rhp - lhp;
            Vector3 disprl = -displr;

            // The positive side of the plane always faces in the selection range.
            LHandPlane = new Plane(displr, lhp);
            RHandPlane = new Plane(disprl, rhp);

            // Now calculate the outward selection plane starting at a particular LERP between the lhp and the rhp vectors.
            Vector3 lerped = Vector3.Lerp(lhp, rhp, 0.5f);

            // Calculate our outwards selection plane
            OutSelPlane = new Plane(lerped, lerped);

            // Update visualization positions
            if(lRSelVis && rRSelVis)
            {
                lRSelVis.transform.position = lhp;
                rRSelVis.transform.position = rhp;

                lRSelVis.transform.rotation = LeftHand.transform.rotation;
                rRSelVis.transform.rotation = RightHand.transform.rotation;
            }
        }

        public void OnGestureEnd()
        {
            // Check if we are not in gesture mode. If so, then turn off the gesture in position check.
            if(!GestureMode.Instance.IsGestureMode())
            {
                GestureInPosition = false;
            }

            if(GestureInPosition)
            {
                // Gesture complete once this flag activates.
                GestureInPosition = false;

                // Get the average rotation of the hands from the top.
                // Note that the right hand's thumb faces backwards and needs an adjustment.
                // (We are trying to get a thumbs up and down configuration.)
                float rotAvg = 
                    (
                        Mathf.Abs(
                            Vector3.Angle(LeftHand.transform.forward, Vector3.up)
                        ) +
                        Mathf.Abs(
                            Vector3.Angle(-RightHand.transform.forward, Vector3.up)
                        )
                    ) / 2;

                Debug.Log("Rot avg: " + rotAvg);

                bool deselMode = rotAvg > 90;

                // Activate all data points inside all three regions.
                foreach (DataPoint point in FindObjectsOfType<DataPoint>())
                {
                    Vector3 ppos = point.transform.position;

                    if(LHandPlane.GetSide(ppos) && RHandPlane.GetSide(ppos) && OutSelPlane.GetSide(ppos))
                    {
                        // Select the point.
                        if(deselMode)
                        {
                            point.Deselect();
                        }
                        else
                        {
                            point.Select();
                        }
                    }
                }

                if(deselMode)
                {
                    UISoundSystem.PlayS(OnDeselectClip);
                }
                else
                {
                    UISoundSystem.PlayS(OnCompleteClip);
                }
            }

            if (lRSelVis && rRSelVis)
            {
                lRSelVis.SetActive(false);
                rRSelVis.SetActive(false);
            }
        }

        // Ends the gesture immediately without changing things
        protected void OnGestureForceEnd()
        {
            GestureInPosition = false;
            OnGestureEnd();
        }
    }
}
