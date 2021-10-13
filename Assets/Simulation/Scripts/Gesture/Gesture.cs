using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace MDVM.Gesture
{
    public class Gesture : MonoBehaviour
    {
        // File names for the gesture capture system.
        public string HandLeftJSON;
        public string HandRightJSON;

        // If not found, direct injection
        public string HandLeftJSONDirect;
        public string HandRightJSONDirect;

        public HandData HandLeftData;

        public HandData HandRightData;

        // References to the gesture capture UI, which will be used to inform the gatherer.
        public GestureCaptureUI CaptureUI;

        bool LeftHandWasMatching = false;
        bool RightHandWasMatching = false;

        public float MatchSetValue = 0.95f;
        public float MatchRetentionValue = 0.90f;

        // Start is called before the first frame update
        void Start()
        {
            GestureInit();
        }

        // Update is called once per frame
        void Update()
        {
            PerformCheck();
        }

        protected void GestureInit()
        {
            // Find a capture UI in the scene and load it.
            if (!CaptureUI)
            {
                CaptureUI = FindObjectOfType<GestureCaptureUI>();
            }

            // Load the gestures from the path.
            LoadHandGestures();
        }

        // Better call this function before doing anything else here!
        protected void PerformCheck()
        {
            if (GestureMode.Instance.IsGestureMode())
            {
                try
                {
                    CheckHandGesture();
                }
                catch (System.InvalidOperationException e)
                {
                    // Do nothing. We are likely not in the right mode for capture. In that case we just ignore.
                    Debug.LogWarning("Error found when activating gesture: " + e);
                }
            }
            else
            {
                // End gestures when out of gesture mode if applicable.
                if(LeftHandWasMatching)
                {
                    OnHandGestureEnd(LeftHand);
                    LeftHandWasMatching = false;
                }
                if (RightHandWasMatching)
                {
                    OnHandGestureEnd(RightHand);
                    RightHandWasMatching = false;
                }
            }
        }

        protected HandData LoadGesture(string path, string direct = null)
        {
            string jsonText = direct;
            try
            {
                if(path != null)
                    jsonText = File.ReadAllText(path);
            }
            catch(FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }

            HandData data = JsonUtility.FromJson<HandData>(jsonText);

            Debug.Log("Data on Gesture read: " + JsonUtility.ToJson(data));

            return data;
        }

        protected virtual void LoadHandGestures()
        {
            HandLeftData = LoadGesture(HandLeftJSON, HandLeftJSONDirect);
            HandRightData = LoadGesture(HandRightJSON, HandRightJSONDirect);
        }

        protected virtual void CheckHandGesture()
        {
            GestureDataGatherer dataGatherer = CaptureUI.dataGather;
            // Get the current hand state, by gathering gesture hand data.
            HandData currentLeftHandData = dataGatherer.CaptureHandData(CaptureUI.LeftHand);
            HandData currentRightHandData = dataGatherer.CaptureHandData(CaptureUI.RightHand);

            float LeftHandSet = dataGatherer.GetMatch(HandLeftData, currentLeftHandData);
            float RightHandSet = dataGatherer.GetMatch(HandRightData, currentRightHandData);

            // Activate particular hand gestures.
            // Left hand.
            if (!LeftHandWasMatching && (LeftHandSet >= MatchSetValue))
            {
                OnHandGesture(LeftHand);
                LeftHandWasMatching = true;
            }
            else if(LeftHandWasMatching && (LeftHandSet <= MatchRetentionValue))
            {
                OnHandGestureEnd(LeftHand);
                LeftHandWasMatching = false;
            }

            // Right hand.
            if (!RightHandWasMatching && (RightHandSet >= MatchSetValue))
            {
                OnHandGesture(RightHand);
                RightHandWasMatching = true;
            }
            else if (RightHandWasMatching && (RightHandSet <= MatchRetentionValue))
            {
                OnHandGestureEnd(RightHand);
                RightHandWasMatching = false;
            }
        }

        protected virtual void OnHandGesture(OVRHand hand)
        {
            // On the specific hand gesture activated.

        }

        protected virtual void OnHandGestureEnd(OVRHand hand)
        {
            // On the specific hand gesture activated.

        }

        public OVRHand LeftHand
        {
            get
            {
                if (CaptureUI)
                    return CaptureUI.LeftHand;
                else return null;
            }
        }

        public OVRHand RightHand
        {
            get
            {
                if (CaptureUI)
                    return CaptureUI.RightHand;
                else return null;
            }
        }

        public bool LeftHandMatching
        {
            get
            {
                return LeftHandWasMatching;
            }
        }

        public bool RightHandMatching
        {
            get
            {
                return RightHandWasMatching;
            }
        }
    }

}
