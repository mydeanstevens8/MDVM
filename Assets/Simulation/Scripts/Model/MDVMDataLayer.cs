using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM.Model {
    public class MDVMDataLayer : MDVMControl
    {
        // Start is called before the first frame update
        void Start()
        {
            Refresh();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Refresh()
        {
            if(plotReference.Bridge)
            {
                if(plotReference.VisualisationController)
                {
                    RawRefresh();
                }
            }
        }

        protected void RawRefresh()
        {

        }
    }
}

