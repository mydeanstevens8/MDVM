using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM.Model
{
    public class MDVMControl : MonoBehaviour
    {
        [HideInInspector]
        protected MDVMPlot plotReference = null;

        protected void SetMDVMPlot(MDVMPlot plot)
        {
            plotReference = plot;
        }

        private void Awake()
        {
            ResetMDVMPlot();
        }

        public void ResetMDVMPlot()
        {
            // Set our plot reference.
            SetMDVMPlot(GetComponentInParent<MDVMPlot>());

            if (plotReference == null)
            {
                Debug.LogWarning("Warning: No MDVM plot reference found.", this);
            }
        }

        public virtual void OnControlStart()
        {

        }

        public virtual void OnControlUpdate()
        {

        }
        public virtual void OnControlDestroy()
        {

        }
    }
}

