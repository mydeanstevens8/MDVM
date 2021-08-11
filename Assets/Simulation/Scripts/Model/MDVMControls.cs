using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM.Model
{
    public class MDVMControls : MonoBehaviour
    {
        public void DestroyControls()
        {
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
        }

        public GameObject CreateControl(GameObject original)
        {
            GameObject newOb = Instantiate(original, transform);
            newOb.name = "MDVMControl: " + newOb.name;

            return newOb;
        }
    }
}

