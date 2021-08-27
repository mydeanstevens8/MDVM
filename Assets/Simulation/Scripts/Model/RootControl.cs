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


namespace MDVM.Model
{
    public class RootControl : MDVMControl
    {
        [System.Serializable]
        public enum RootControlMode
        {
            Move,
            Rotate,
            Details,
            Metadata
        }

        public Mesh moveMesh = null;
        public Mesh rotateMesh = null;
        public Mesh detailsMesh = null;
        public Mesh metadataMesh = null;

        MeshFilter rootMeshFilter = null;
        MeshRenderer rootMeshRenderer = null;
        MyGrabbable grabbableRoot = null;

        RootControlMode m_mode = RootControlMode.Move;

        // Start is called before the first frame update
        void Start()
        {
            rootMeshFilter = GetComponent<MeshFilter>();
            rootMeshRenderer = GetComponent<MeshRenderer>();
            grabbableRoot = GetComponent<MyGrabbable>();

            if(grabbableRoot)
            {
                grabbableRoot.SetObjectToApplyGrab(plotReference.gameObject);
            }
        }

        public RootControlMode Mode
        {
            get
            {
                return m_mode;
            }
            set
            {
                m_mode = value;

                MyGrabbable.ControlMode grabMode = MyGrabbable.ControlMode.Locked;

                if(rootMeshRenderer != null)
                {
                    switch(value)
                    {
                        case RootControlMode.Move:
                            rootMeshFilter.mesh = moveMesh;
                            grabMode = MyGrabbable.ControlMode.Move;
                            break;
                        case RootControlMode.Rotate:
                            rootMeshFilter.mesh = rotateMesh? rotateMesh : moveMesh;
                            grabMode = MyGrabbable.ControlMode.Rotate;
                            break;
                        case RootControlMode.Details:
                            rootMeshFilter.mesh = detailsMesh ? detailsMesh : moveMesh;
                            break;
                        case RootControlMode.Metadata:
                            rootMeshFilter.mesh = metadataMesh ? metadataMesh : moveMesh;
                            break;
                    }

                    if(grabbableRoot != null)
                    {
                        grabbableRoot.mode = grabMode;
                    }
                }
            }
        }

        // To be called on Unity events since they can't access properties, but otherwise the same as setting the property Mode itself
        public void SetModeMove()
        {
            Mode = RootControlMode.Move;
        }

        public void SetModeRotate()
        {
            Mode = RootControlMode.Rotate;
        }

        public void SetModeDetails()
        {
            Mode = RootControlMode.Details;
        }
        public void SetModeMetadata()
        {
            Mode = RootControlMode.Metadata;
        }
        public void SetGrabbableRotation(float yEDeg) 
        { 
            if(grabbableRoot)
            {
                grabbableRoot.SetRotationThroughGrabbable(Quaternion.Euler(0, yEDeg, 0));
            }
        }
    }
}