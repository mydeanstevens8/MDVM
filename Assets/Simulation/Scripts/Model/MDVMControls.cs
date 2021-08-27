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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM.Model
{
    public class MDVMControls : MonoBehaviour
    {
        protected List<MDVMControl> controls = new List<MDVMControl>();

        public void DestroyControls()
        {
            DestroyControlsEventCall();

            List<GameObject> objectsToDestroy = new List<GameObject>();
            // This strange undocumented feature allows us to loop over all children
            foreach (Transform child in transform)
            {
                // Destroy all children, destroying their children in turn, if we have an MDVM control
                if(child.GetComponent<MDVMControl>() != null)
                {
                    // Do not destroy right now. Wait for this loop to finish and then destroy them later.
                    objectsToDestroy.Add(child.gameObject);
                }
            }

            // Now we will destroy them.
            foreach(GameObject toDestroy in objectsToDestroy)
            {
                if (Application.isPlaying)
                {
                    Destroy(toDestroy);
                }
                else
                {
                    DestroyImmediate(toDestroy);
                }
            }

            controls.Clear();
        }

        public GameObject CreateControl(GameObject original)
        {
            GameObject newOb = Instantiate(original, transform);
            newOb.name = "MDVMControl: " + newOb.name;

            MDVMControl refControl = newOb.GetComponent<MDVMControl>();

            if(refControl)
            {
                controls.Add(refControl);
            }

            return newOb;
        }

        public GameObject CreateControlRaw(Type controlType)
        {
            GameObject newOb = new GameObject("MDVMControl: " + controlType.Name, controlType);
            newOb.transform.parent = transform;

            Component controlOb = newOb.GetComponent(controlType);

            if(controlOb is MDVMControl)
            {
                MDVMControl controlObMDVM = (MDVMControl) controlOb;

                controlObMDVM.ResetMDVMPlot();
                controls.Add(controlObMDVM);
            }
            
            return newOb;
        }

        public MDVMControl[] Controls
        {
            get
            {
                return controls.ToArray();
            }
        }

        internal void StartControls()
        {
            foreach (MDVMControl control in Controls)
            {
                control.OnControlStart();
            }
        }

        internal void UpdateControls()
        {
            foreach (MDVMControl control in Controls)
            {
                control.OnControlUpdate();
            }
        }

        internal void DestroyControlsEventCall()
        {
            foreach (MDVMControl control in Controls)
            {
                control.OnControlDestroy();
            }
        }
    }
}

