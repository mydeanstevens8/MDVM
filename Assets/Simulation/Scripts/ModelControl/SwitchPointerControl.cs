using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MDVM.UI
{
    [RequireComponent(typeof(OVRInputModule))]
    [DisallowMultipleComponent]
    public class SwitchPointerControl : MonoBehaviour
    {
        public List<OVRRaycaster> worldRaycasters = new List<OVRRaycaster>();

        private OVRInputModule inModule = null;

        private static SwitchPointerControl instance = null;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            inModule = GetComponent<OVRInputModule>();
        }

        public void SwitchPointer(GameObject pointerObject)
        {
            foreach(OVRRaycaster myRaycaster in worldRaycasters)
            {
                myRaycaster.pointer = pointerObject;
            }

            inModule.rayTransform = pointerObject.transform;
        }

        public GameObject GetCurrentPointer()
        {
            return inModule.rayTransform.gameObject;
        }

        public bool IsLeftPointerActive()
        {
            GameObject pointer = GetCurrentPointer();

            MyPointBehaviour pointBehaviour = pointer.GetComponent<MyPointBehaviour>();

            if(pointBehaviour != null)
            {
                return pointBehaviour.LeftHand;
            }

            // Assume not if we can't find it.
            return false;
        }

        public static SwitchPointerControl Get()
        {
            return instance;
        }
    }
}