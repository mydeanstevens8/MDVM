using System.Collections.Generic;
using UnityEngine;

namespace MDVM.Gesture
{
    [System.Serializable]
    public struct HandData
    {
        public string Name;

        public OVRHand.TrackingConfidence Confidence;
        public OVRSkeleton.SkeletonType SkeletonType;
        public Dictionary<OVRSkeleton.BoneId, Vector3> bonePositions;

        // For gesture calculation.
        public Dictionary<OVRSkeleton.BoneId, float> boneWeights;
    }
}
