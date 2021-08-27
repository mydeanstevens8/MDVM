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

using System.Collections;
using UnityEngine;

namespace MDVM.Gesture
{
    public class GestureDataGatherer : MonoBehaviour
    {
        public OVRHand handL = null;
        public OVRHand handR = null;

        public float matchThreshold = 0.95f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public HandData CaptureHandData(OVRHand hand)
        {
            // Capture the data and put it into a script.
            // Normalize the data before processing

            // !!! WARNING !!!
            // DO NOT USE THIS DATA OUTSIDE OF GESTURE TRACKING PURPOSES
            // AS THIS COULD RESULT IN A DEV BAN.
            // This is only used to normalise the data that we receive.
            float handScale = hand.HandScale;

            HandData myData = new HandData();

            myData.Name = System.Guid.NewGuid().ToString();

            myData.Confidence = hand.HandConfidence;

            // Should exist
            OVRSkeleton skeleton = hand.GetComponent<OVRSkeleton>();

            myData.SkeletonType = skeleton.GetSkeletonType();

            // Calculate the starting vectors we need for extrapolation.
            OVRBone handStart = skeleton.Bones[(int) OVRPlugin.BoneId.Hand_Start];
            OVRBone handMidStart = skeleton.Bones[(int) OVRPlugin.BoneId.Hand_Middle1];
            OVRBone handThumbStart = skeleton.Bones[(int)OVRPlugin.BoneId.Hand_Thumb0];

            Vector3 root = handStart.Transform.position;

            Vector3 localForward = (handMidStart.Transform.position - root).normalized;
            Vector3 localThumb = (handThumbStart.Transform.position - root).normalized;

            // From the right hand's perspective. Flipped for the left hand.
            Vector3 localRight = -(localThumb - Vector3.Project(localThumb, localForward)).normalized;

            if(myData.SkeletonType == OVRSkeleton.SkeletonType.HandLeft)
            {
                // Flip it so that it is in the right direction.
                localRight = -localRight;
            }

            // The cross product going up out of the local forward and the local right
            // Unity uses a left hand coordinate system: Forward and then right to create y.
            Vector3 localUp = Vector3.Cross(localForward, localRight).normalized;

            Quaternion localRotation = Quaternion.LookRotation(localForward, localUp);
            Matrix4x4 localCoords = Matrix4x4.TRS(root, localRotation, handScale * Vector3.one);

            foreach (OVRBone bone in skeleton.Bones)
            {
                OVRSkeleton.BoneId boneID = bone.Id;

                Vector3 bonePos = bone.Transform.position;

                Vector3 normalizedBonePos = localCoords.MultiplyVector(bonePos);

                myData.bonePositions.Add(boneID, normalizedBonePos);
            }

            return myData;
        }


        public float GetMatch(HandData reference, HandData compare)
        {
            // Get all the bones and their matches with the second.
            // Use a weighted average, and if a weight does not exist,
            // assume that the weight is 1.

            // The weights of the reference are used for the computation.

            float matchConfidence = 0;

            float dividend = 0;

            var refPositions = reference.bonePositions;
            var cmpPositions = compare.bonePositions;

            foreach(var entry in refPositions)
            {
                // Compare the weights of the bones that exist in the other hand
                if(cmpPositions.ContainsKey(entry.Key))
                {
                    Vector3 boneRefPos = entry.Value;
                    Vector3 boneCmpPos = cmpPositions[entry.Key];

                    float localWeight = 1.0f;

                    if(reference.boneWeights.ContainsKey(entry.Key))
                    {
                        localWeight = reference.boneWeights[entry.Key];
                    }

                    float localSimilarity = localWeight * (Vector3.Dot(boneRefPos, boneCmpPos) + 1) / 2;

                    matchConfidence += localSimilarity;
                    dividend += localWeight;
                }
            }

            return matchConfidence / dividend;
        }

        public bool Matches(HandData reference, HandData compare)
        {
            return GetMatch(reference, compare) >= matchThreshold;
        }
    }
}
