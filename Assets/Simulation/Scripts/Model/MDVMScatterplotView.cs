using System;
using System.Collections.Generic;
using UnityEngine;
using IATK;

namespace MDVM.Model
{
    public class MDVMScatterplotView : MDVMView
    {
        protected View viewReference = null;

        protected long uniqueCreationIDStart = 0;

        public override void ClearViewLayer(MDVMDataLayer layer)
        {

        }

        public override void GenerateViewLayer(MDVMDataLayer layer, View view)
        {
            viewReference = view;

            RefillPoints(layer);
        }

        public override void UpdateViewLayer(MDVMDataLayer layer)
        {
            if(Application.isPlaying)
            {
                RefillPoints(layer);
            }
        }

        protected void RefillPoints(MDVMDataLayer layer)
        {
            // Clear children first.
            List<Transform> childrenToClear = new List<Transform>();

            foreach(Transform child in transform)
            {
                childrenToClear.Add(child);
            }

            while(childrenToClear.Count > 0)
            {
                Transform child = childrenToClear[0];
                childrenToClear.RemoveAt(0);

                Destroy(child.gameObject);
            }

            // Grab the vertices from the view and add them
            Vector3[] allVerts = viewReference.GetPositions();

            Vector3 graphScale = Vector3.one;

            MDVMPlot plotRef = layer.GetPlotReference();

            if(plotRef != null && plotRef.Bridge != null && plotRef.Bridge.VisualisationController != null)
            {
                Visualisation IATKVis = plotRef.VisualisationController;

                graphScale = new Vector3(IATKVis.width, IATKVis.height, IATKVis.depth);
            }

            // Copy our point template
            if(pointTemplate != null)
            {
                foreach(Vector3 vert in allVerts)
                {
                    GameObject newPoint = Instantiate(pointTemplate, transform);

                    // Set points
                    newPoint.transform.localPosition = new Vector3(vert.x * graphScale.x, vert.y * graphScale.y, vert.z * graphScale.z);
                    newPoint.transform.localRotation = Quaternion.identity;

                    DataPoint pointData = newPoint.GetComponent<DataPoint>();

                    if(pointData)
                    {
                        pointData.dataPointReference = vert;
                        pointData.uniqueCreationID = uniqueCreationIDStart++;
                    }

                }
            }
        }
    }
}
