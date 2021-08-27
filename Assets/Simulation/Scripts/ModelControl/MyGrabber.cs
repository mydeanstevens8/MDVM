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

using UnityEngine;

namespace MDVM
{

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