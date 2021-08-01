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
            MDVMPlot oldPlot = GetComponent<MDVMPlot>();
            T newPlot;

            if (oldPlot != null && oldPlot is T)
            {
                newPlot = oldPlot as T;
            }
            else
            {
                DestroyOldPlotComponent();
                newPlot = gameObject.AddComponent<T>();
            }
            // Create the new MDVM plot
            return newPlot;
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

        private void DestroyOldPlotComponent()
        {
            MDVMPlot oldPlot = GetComponent<MDVMPlot>();
            if (oldPlot != null)
            {
                // Since we can run in the editor, we can't use Destroy.
                DestroyImmediate(oldPlot);
            }
        }

        private void UpdatePlotComponent()
        {
            // Create an MDVM plot
            MDVMPlot newPlot = null;

            switch (IATKVisualisation.visualisationType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                    newPlot = AddMDVMPlot<MDVMScatterPlot>();
                    break;
                default:
                    // Destroy all plot components if we don't know.
                    DestroyOldPlotComponent();
                    Debug.LogWarning("Unrecognized visualisation type from IATK: " + IATKVisualisation.visualisationType, this);
                    break;
            }

            if (newPlot != null)
            {
                newPlot.Bridge = this;
                newPlot.MDVMStart();
                newPlot.SetUpMDVMPlot();
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

