using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;


namespace MDVM.Model
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MDVMToIATKBridge))]
    public abstract class MDVMPlot : MonoBehaviour
    {
        private MDVMToIATKBridge bridge = null;

        protected GameObject controlsLayer = null;

        public MDVMToIATKBridge Bridge
        {
            get
            {
                return bridge;
            }
            internal set
            {
                bridge = value;
            }
        }

        public Visualisation VisualisationController
        {
            get
            {
                return Bridge.VisualisationController;
            }
        }

        internal void MDVMStart()
        {

        }

        public virtual void SetUpMDVMPlot()
        {

        }
    }

}
