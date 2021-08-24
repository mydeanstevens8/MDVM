using System;
using System.Collections;
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

