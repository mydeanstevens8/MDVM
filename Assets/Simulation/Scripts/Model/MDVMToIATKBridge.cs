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
        public string uid = "MDVM1";

        public bool refresh = false;
        protected Visualisation IATKVisualisation = null;

        protected MDVMPlot mPlot = null;


        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("MDVM is initializing.", this);
            InitializeObject();
        }

        // Update is called once per frame
        void Update()
        {
            if(refresh)
            {
                Debug.Log("MDVM has refreshed.", this);
                refresh = false;
                RefreshMDVM();
            }
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

            if(IATKVisualisation == null)
            {
                Debug.LogError("No IATK Visualisation detected in a child. ", this);
                return;
            }

            IATKVisualisation.uid = uid;

            // Ensure that the data is loaded
            DataSource dataSource = IATKVisualisation.dataSource;

            if(!dataSource.IsLoaded)
            {
                Debug.Log("Loading a data source since data is not loaded.", this);
                dataSource.load();
            }

            if(IATKVisualisation.theVisualizationObject == null)
            {
                Debug.Log("Creating a visualisation from the visualiser.", this);
                // Create the visualisation if it does not exist.
                IATKVisualisation.CreateVisualisation(IATKVisualisation.visualisationType);
                IATKVisualisation.updateProperties();
            }
            else
            {
                Debug.Log("Recreating the visualisation from the visualiser.", this);
            }

            UpdatePlotComponent();
        }

        private void DestroyOldPlotComponent()
        {
            MDVMPlot oldPlot = GetComponent<MDVMPlot>();
            if (oldPlot != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(oldPlot);
                }
                else
                {
                    // Editor does not support destroy when editing.
                    DestroyImmediate(oldPlot);
                }
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

