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

            GrabbedObjectRotation = Quaternion.Inverse(ReferencePoint.transform.rotation) * GrabbedObject.transform.rotation;

            GrabbedObject.GetComponent<MyGrabbable>().GrabbableStart(
                this, ReferencePoint.transform, GrabbedObjectDisplacement, GrabbedObjectRotation
                );
        }

        public void EndGrab()
        {
            // End grab may be called with a null object.
            if(GrabbedObject != null)
            {
                GrabbedObject.GetComponent<MyGrabbable>().GrabbableEnd(
                this, ReferencePoint.transform, GrabbedObjectDisplacement, GrabbedObjectRotation
                );
            }

            GrabbedObject = null;
            GrabbedObjectDisplacement = new Vector3();
            GrabbedObjectRotation = Quaternion.identity;
        }

        protected void GrabUpdate()
        {
            if(GrabbedObject != null)
            {
                // Call our delegate which will perform our grab update for us.
                GrabbedObject.GetComponent<MyGrabbable>().GrabbableUpdate(
                    this, ReferencePoint.transform, GrabbedObjectDisplacement, GrabbedObjectRotation
                    );
            }
        }

    }

}