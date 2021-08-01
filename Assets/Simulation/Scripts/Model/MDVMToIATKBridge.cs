using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IATK;

namespace MDVM.Model
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class MDVMToIATKBridge : MonoBehaviour
    {
        public bool refresh = false;
        protected Visualisation IATKVisualisation = null;

        protected MDVMPlot mPlot = null;


        // Start is called before the first frame update
        void Start()
        {
            InitializeObject();
        }

        // Update is called once per frame
        void Update()
        {
            if(refresh)
            {
                RefreshMDVM();
                refresh = false;
            }
        }

        private void OnEnable()
        {
            InitializeObject();
        }

        private void OnValidate()
        {
            // Don't refresh here. Wait until the user clicks the refresh button.
        }

        private T AddMDVMPlot<T>() where T : MDVMPlot
        {
            // Create the new MDVM plot
            return gameObject.AddComponent<T>();
        }

        public void RefreshMDVM()
        {
            InitializeObject();
        }

        private void InitializeObject()
        {
            ConnectToIATK();
        }

        private void ConnectToIATK()
        {
            IATKVisualisation = GetComponentInChildren<Visualisation>();

            UpdatePlotComponent();
        }

        private void UpdatePlotComponent()
        {
            // If an existing MDVM component exists, destroy it.
            MDVMPlot oldPlot = GetComponent<MDVMPlot>();
            if (oldPlot != null)
            {
                // Since we can run in the editor, we can't use Destroy.
                DestroyImmediate(oldPlot);
            }

            // Create an MDVM plot
            MDVMPlot newPlot = null;

            switch (IATKVisualisation.visualisationType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                    newPlot = AddMDVMPlot<MDVMScatterPlot>();
                    break;
                default:
                    break;
            }

            if (newPlot != null)
            {
                newPlot.Bridge = this;
            }
        }

        public Visualisation VisualisationController
        {
            get
            {
                return IATKVisualisation;
            }
        }


        public MDVMPlot Plot
        {
            get
            {
                return mPlot;
            }
        }
    }
}

