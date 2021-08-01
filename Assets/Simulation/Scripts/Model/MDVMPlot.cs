using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MDVM.Model
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MDVMToIATKBridge))]
    public abstract class MDVMPlot : MonoBehaviour
    {
        private MDVMToIATKBridge bridge = null;

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
    }

}
