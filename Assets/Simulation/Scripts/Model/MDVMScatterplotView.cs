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
