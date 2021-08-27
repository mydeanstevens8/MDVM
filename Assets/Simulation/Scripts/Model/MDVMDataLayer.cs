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

using System;
using System.Collections.Generic;
using UnityEngine;
using IATK;

namespace MDVM.Model {

    [ExecuteInEditMode]
    public abstract class MDVMDataLayer : MDVMControl
    {
        // Template for views.
        [SerializeField]
        protected internal GameObject viewTemplate = null;

        [SerializeField]
        protected internal GameObject pointTemplate = null;

        List<MDVMView> viewObjects = new List<MDVMView>();

        // Start is called before the first frame update
        public override void OnControlStart()
        {
            Refresh();
        }

        // Update is called once per frame
        public override void OnControlUpdate()
        {
            UpdateDataLayer();
        }

        public void Refresh()
        {
            if(plotReference && plotReference.Bridge)
            {
                if(plotReference.VisualisationController)
                {
                    RawRefresh();
                }
            }
        }

        protected virtual void RawRefresh()
        {
            ClearDataLayer();
            PopulateDataLayer();
        }

        protected virtual void ClearDataLayer()
        {
            List<Transform> children = new List<Transform>();
            foreach(Transform childTransform in transform)
            {
                children.Add(childTransform);
            }

            while(children.Count > 0)
            {
                Transform child = children[0];
                children.RemoveAt(0);

                MDVMView myView = child.GetComponent<MDVMView>();
                if(myView != null)
                {
                    myView.ClearViewLayer(this);
                }

                if(Application.isEditor && !Application.isPlaying)
                {
                    DestroyImmediate(child.gameObject);
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }

            viewObjects.Clear();
        }

        protected virtual void PopulateDataLayer()
        {
            Visualisation controller = plotReference.VisualisationController;

            AbstractVisualisation visOb = controller.theVisualizationObject;

            if(visOb != null)
            {
                foreach(View v in visOb.viewList)
                {
                    PopulateViewUI(v);
                }
            }
        }

        protected virtual void PopulateViewUI(View view)
        {
            GameObject newViewObj;

            if(viewTemplate != null)
            {
                newViewObj = Instantiate(viewTemplate);
            } 
            else
            {
                newViewObj = new GameObject("MDVM View: " + view.name);
            }

            // Set parent.
            newViewObj.transform.parent = transform;

            MDVMView newView = newViewObj.GetComponent<MDVMView>();

            if(newView == null)
            {
                newView = (MDVMView) newViewObj.AddComponent(GetViewType());
                newView.pointTemplate = pointTemplate;
            }

            newView.GenerateViewLayer(this, view);


            viewObjects.Add(newView);

            // Make sure that our local positions are normalized
            newViewObj.transform.localPosition = Vector3.zero;
            newViewObj.transform.rotation = Quaternion.identity;
        }

        protected virtual void UpdateDataLayer()
        {
            foreach(MDVMView myView in viewObjects)
            {
                myView.UpdateViewLayer(this);
            }
        }

        public abstract Type GetViewType();

        public MDVMPlot GetPlotReference()
        {
            return plotReference;
        }
    }
}

