using UnityEngine;
using System.Collections;

using MDVM.Model;

namespace MDVM.Gesture
{
    class PointGesture : Gesture
    {
        public GameObject pointerPrefabL;
        public GameObject pointerPrefabR;

        protected GameObject pointerPrefabInstanceL;
        protected GameObject pointerPrefabInstanceR;

        Vector3 PositionPointerL;
        Vector3 DirectionPointerL;

        Vector3 PositionPointerR;
        Vector3 DirectionPointerR;

        // Collision primitives for the left and the right hand
        Collider LPCollider;
        Collider RPCollider;

        // Hovering point for the left hand.
        DataPoint LPHovering;
        DataPoint RPHovering;

        // Flag for collision
        bool LPActivated = false;
        bool RPActivated = false;

        bool LSelectCoroutineFinished = true;
        bool RSelectCoroutineFinished = true;

        // Start is called before the first frame update
        void Start()
        {
            GestureInit();

            if(pointerPrefabL)
            {
                pointerPrefabInstanceL = Instantiate(pointerPrefabL, transform);
                LPCollider = GetComponentInChildren<Collider>();

                pointerPrefabInstanceL.SetActive(false);
            }

            if (pointerPrefabR)
            {
                pointerPrefabInstanceR = Instantiate(pointerPrefabR, transform);
                RPCollider = GetComponentInChildren<Collider>();

                pointerPrefabInstanceR.SetActive(false);
            }
        }

        private void Update()
        {
            // Must call this.
            PerformCheck();

            // Update positions when required.
            if (LeftHandMatching)
            {
                SetPointerPosition(LeftHand, out PositionPointerL, out DirectionPointerL);

                // Update prefab positions.
                if(pointerPrefabInstanceL)
                {
                    UpdatePointerPrefab(pointerPrefabInstanceL, PositionPointerL, DirectionPointerL);
                }

                LPHovering = CheckCollisions(LPCollider);

                if(LPHovering != null)
                {
                    if(LSelectCoroutineFinished)
                    {
                        LSelectCoroutineFinished = false;
                        StartCoroutine(SelectionCoroutine(LeftHand, LPHovering, pointerPrefabInstanceL, LPCollider, LPActivated));
                    }
                }
            }
            else
            {
                LPHovering = null;
            }
            
            if (RightHandMatching)
            {
                SetPointerPosition(RightHand, out PositionPointerR, out DirectionPointerR);

                // Update prefab positions.
                if (pointerPrefabInstanceR)
                {
                    UpdatePointerPrefab(pointerPrefabInstanceR, PositionPointerR, DirectionPointerR);
                }

                RPHovering = CheckCollisions(RPCollider);

                if (RPHovering != null)
                {
                    if(RSelectCoroutineFinished)
                    {
                        RSelectCoroutineFinished = false;
                        StartCoroutine(SelectionCoroutine(RightHand, RPHovering, pointerPrefabInstanceR, RPCollider, RPActivated));
                    }
                }
            }
            else
            {
                RPHovering = null;
            }
        }

        protected override void OnHandGesture(OVRHand hand)
        {
            // On the specific hand gesture activated.
            // Activate on start, given the hand.
            if(hand == LeftHand && pointerPrefabInstanceL)
            {
                pointerPrefabInstanceL.SetActive(true);
            }

            if (hand == RightHand && pointerPrefabInstanceR)
            {
                pointerPrefabInstanceR.SetActive(true);
            }
        }

        protected override void OnHandGestureEnd(OVRHand hand)
        {
            // On the specific hand gesture activated.
            // Deactivate on end, given the hand.
            if (hand == LeftHand && pointerPrefabInstanceL)
            {
                pointerPrefabInstanceL.SetActive(false);
            }

            if (hand == RightHand && pointerPrefabInstanceR)
            {
                pointerPrefabInstanceR.SetActive(false);
            }
        }

        protected void SetPointerPosition(OVRHand reference, out Vector3 position, out Vector3 direction)
        {
            // Should exist
            OVRSkeleton skeleton = reference.GetComponent<OVRSkeleton>();

            OVRBone IndexFingerStart = null;
            OVRBone IndexFingerMiddle = null;
            OVRBone IndexFingerEnd = null;
            OVRBone IndexFingerTip = null;

            // Get the index finger bones of our reference and use it to position our pointer.
            foreach (OVRBone bone in skeleton.Bones)
            {
                if (bone.Id == OVRSkeleton.BoneId.Hand_Index1) IndexFingerStart = bone;
                if (bone.Id == OVRSkeleton.BoneId.Hand_Index2) IndexFingerMiddle = bone;
                if (bone.Id == OVRSkeleton.BoneId.Hand_Index3) IndexFingerEnd = bone;
                if (bone.Id == OVRSkeleton.BoneId.Hand_IndexTip) IndexFingerTip = bone;
            }

            if(IndexFingerStart != null && IndexFingerMiddle != null && IndexFingerEnd != null && IndexFingerTip != null)
            {
                // Use them to create a position vector.
                Vector3 PointerDirection = 
                    2 * (IndexFingerTip.Transform.position - IndexFingerEnd.Transform.position) +
                    1.5f * (IndexFingerEnd.Transform.position - IndexFingerMiddle.Transform.position) +
                    1 * (IndexFingerMiddle.Transform.position - IndexFingerStart.Transform.position);

                // Load the starting position vector from the tip
                Vector3 StartingPosition = IndexFingerTip.Transform.position;

                position = StartingPosition;
                direction = PointerDirection.normalized;
                return;
            }

            // Default position, can't set.
            position = Vector3.zero;
            direction = Vector3.forward;
        }

        protected void UpdatePointerPrefab(GameObject pointerPrefab, Vector3 position, Vector3 direction)
        {
            pointerPrefab.transform.position = position;
            pointerPrefab.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        // Collision events will not happen when two triggers collide with each other, so we will check it manually.
        protected DataPoint CheckCollisions(Collider col)
        {
            DataPoint closest = null;
            float closestDist = Mathf.Infinity;

            foreach (DataPoint point in FindObjectsOfType<DataPoint>())
            {
                if (col.ClosestPoint(point.transform.position) == point.transform.position)
                {
                    // Colliding. Get distance from us
                    float dist = Vector3.Distance(col.transform.position, point.transform.position);

                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = point;
                    }
                }
            }

            return closest;
        }

        protected bool CheckHandInDataPoint(OVRHand hand, DataPoint point)
        {
            // Should exist
            OVRSkeleton skeleton = hand.GetComponent<OVRSkeleton>();

            OVRBone tipBone = null;
            foreach (OVRBone bone in skeleton.Bones)
            {
                if (bone.Id == OVRSkeleton.BoneId.Hand_IndexTip) tipBone = bone;
            }

            Collider col = point.GetComponent<Collider>();

            if(tipBone != null)
            {
                return col.bounds.Contains(tipBone.Transform.position);
            }

            return false;
        }

        protected void ActivateSelectionSwitch(OVRHand hand, DataPoint point, bool Activated, out bool NewActivated)
        {
            if (!Activated)
            {
                // Play the action of selecting the point.
                point.ActionSelect();
                NewActivated = true;
            }
            else
            {
                // Play the action of selecting the point.
                point.ActionSelect();
                NewActivated = false;
            }
        }

        protected IEnumerator SelectionCoroutine(OVRHand hand, DataPoint toSelect, GameObject pointerPrefab, Collider col, bool setActivated)
        {
            Debug.Log("Running selection coroutine.");
            float startTime = Time.time;
            float endTime = startTime + 1;

            while(Time.time < endTime)
            {
                if(!pointerPrefab.activeSelf || (CheckCollisions(col) != toSelect))
                {
                    if(hand == LeftHand)
                    {
                        LSelectCoroutineFinished = true;
                    }
                    else
                    {
                        RSelectCoroutineFinished = true;
                    }

                    Debug.Log("Selection coroutine stopped due to wrong point.");
                    yield break;
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            Debug.Log("Selection coroutine succeeded.");

            bool newActivated;
            ActivateSelectionSwitch(hand, toSelect, setActivated, out newActivated);

            if(hand == LeftHand)
            {
                LPActivated = newActivated;
                LSelectCoroutineFinished = true;
            }
            else
            {
                RPActivated = newActivated;
                RSelectCoroutineFinished = true;
            }

            yield return null;
        }
    }
}
