using System;
using System.Collections.Generic;
using UnityEngine;
using IATK;

namespace MDVM.Model
{
    public abstract class MDVMView : MonoBehaviour
    {
        [SerializeField]
        protected internal GameObject pointTemplate = null;
        public abstract void ClearViewLayer(MDVMDataLayer layer);

        public abstract void GenerateViewLayer(MDVMDataLayer layer, View view);

        public abstract void UpdateViewLayer(MDVMDataLayer layer);
    }
}
