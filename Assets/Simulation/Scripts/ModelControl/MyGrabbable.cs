namespace MDVM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MyGrabbable : MonoBehaviour
    {
        public Vector3 LockToXAxis = Vector3.right;
        public Vector3 LockToYAxis = Vector3.up;
        public Vector3 LockToZAxis = Vector3.forward;

        public Vector3 GridSnap = new Vector3();

        public bool LockRotation = false;

        // Whether or not the object is in it's domain.
        // Dragging the object outside the domain should not be allowed.
        // In this case, the domain is R^3.
        public virtual bool InDomain()
        {
            return true;
        }

        public void DoGrab()
        {
            MyGrabberCollection.Get().AttemptGrab(gameObject);
        }

        public void StopGrab()
        {
            MyGrabberCollection.Get().EndAllGrabs();
        }
    }

}