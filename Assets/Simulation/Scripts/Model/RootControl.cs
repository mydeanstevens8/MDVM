using System.Collections;
using System.Collections.Generic;
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