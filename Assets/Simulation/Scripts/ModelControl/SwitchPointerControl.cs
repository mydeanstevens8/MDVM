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

            MyVRInputModule myInMod = inModule as MyVRInputModule;
            if (myInMod != null && pointerObject.GetComponent<MyPointBehaviour>() != null)
            {
                myInMod.leftHandMode = pointerObject.GetComponent<MyPointBehaviour>().LeftHand;
            }
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