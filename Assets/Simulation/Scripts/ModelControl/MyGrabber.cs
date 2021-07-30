namespace MDVM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MyGrabber : MonoBehaviour
    {
        public GameObject ReferencePoint;

        public bool LeftGrabber = false;

        public GameObject GrabbedObject { get; protected set; }

        public Vector3 GrabbedObjectDisplacement { get; protected set; }

        public Quaternion GrabbedObjectRotation { get; protected set; }

        // Start is called before the first frame update
        void Start()
        {
            MyGrabberCollection col = MyGrabberCollection.Get();

            if(LeftGrabber)
            {
                col.RegisterLeft(this);
            }
            else
            {
                col.RegisterRight(this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            GrabUpdate();
        }

        public GameObject BeginGrab(GameObject objToGrab)
        {
            Debug.Log("Begin grab of " + (objToGrab? objToGrab.ToString() : "<nothing in particular>") + " ... ");

            if (objToGrab != null)
            {
                ForceGrab(objToGrab);
            }

            return objToGrab;
        }

        protected void ForceGrab(GameObject myObject)
        {
            GrabbedObject = myObject;
            GrabbedObjectDisplacement = ReferencePoint.transform.InverseTransformPoint(GrabbedObject.transform.position);
            GrabbedObjectRotation = GrabbedObject.transform.rotation * Quaternion.Inverse(ReferencePoint.transform.rotation);
        }

        public void EndGrab()
        {
            Debug.Log("End grab");

            GrabbedObject = null;
            GrabbedObjectDisplacement = new Vector3();
            GrabbedObjectRotation = Quaternion.identity;
        }

        protected void GrabUpdate()
        {
            if(GrabbedObject != null)
            {
                // Apply rotation transforms here.
                GrabbedObject.transform.position = ReferencePoint.transform.TransformPoint(GrabbedObjectDisplacement);
                GrabbedObject.transform.rotation = GrabbedObjectRotation * ReferencePoint.transform.rotation;
            }
        }

    }

}