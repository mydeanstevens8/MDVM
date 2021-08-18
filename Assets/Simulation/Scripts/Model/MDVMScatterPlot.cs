using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM.Model
{
    public class MDVMScatterPlot : MDVMPlot
    {
        public GameObject axisControllerPrefab = null;
        public GameObject rootPrefab = null;

        public GameObject dataViewPrefab = null;

        public override void SetUpMDVMPlot()
        {
            base.SetUpMDVMPlot();
            controlsLayer.DestroyControls();
            CreateScatterplotControls();
        }

        protected void AddAxisControllers()
        {
            if(axisControllerPrefab != null)
            {
                GameObject axisX = controlsLayer.CreateControl(axisControllerPrefab);
                AxisControls axisXControls = axisX.GetComponent<AxisControls>();

                if(axisXControls != null)
                {
                    axisXControls.direction = AxisDirection.X;
                    axisXControls.Refresh();
                }

                GameObject axisY = controlsLayer.CreateControl(axisControllerPrefab);
                AxisControls axisYControls = axisY.GetComponent<AxisControls>();

                if (axisYControls != null)
                {
                    axisYControls.direction = AxisDirection.Y;
                    axisYControls.Refresh();
                }

                GameObject axisZ = controlsLayer.CreateControl(axisControllerPrefab);
                AxisControls axisZControls = axisZ.GetComponent<AxisControls>();

                if (axisZControls != null)
                {
                    axisZControls.direction = AxisDirection.Z;
                    axisZControls.Refresh();
                }
            }
        }

        protected void CreateScatterplotControls()
        {
            // Root
            controlsLayer.CreateControl(rootPrefab);

            // Axis
            AddAxisControllers();

            // View layer
            if (dataViewPrefab == null)
            {
                controlsLayer.CreateControlRaw(typeof(MDVMScatterplotDataLayer));
            }
            else
            {
                controlsLayer.CreateControl(dataViewPrefab);
            }
        }

    }
}
