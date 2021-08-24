using System;
using System.Collections;
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

