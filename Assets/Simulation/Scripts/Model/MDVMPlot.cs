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

using UnityEngine;
using IATK;


namespace MDVM.Model
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MDVMToIATKBridge))]
    public abstract class MDVMPlot : MonoBehaviour
    {
        [System.Serializable]
        public enum AxisDirection
        {
            X,
            Y,
            Z
        }

        private MDVMToIATKBridge bridge = null;

        protected MDVMControls controlsLayer = null;

        public MDVMToIATKBridge Bridge
        {
            get
            {
                return bridge;
            }
            internal set
            {
                bridge = value;
            }
        }

        public Visualisation VisualisationController
        {
            get
            {
                return Bridge.VisualisationController;
            }
        }

        internal void MDVMStart()
        {
            controlsLayer = GetComponentInChildren<MDVMControls>();
            controlsLayer.DestroyControls();

            SetUpMDVMPlot();

            controlsLayer.StartControls();
        }

        private void OnDestroy()
        {
            if(controlsLayer != null)
            {
                controlsLayer.DestroyControls();

                if(Application.isPlaying)
                {
                    Destroy(controlsLayer);
                }
                else
                {
                    DestroyImmediate(controlsLayer);
                }
            }
        }

        public virtual void SetUpMDVMPlot()
        {

        }

        public void MDVMUpdate()
        {
            // Expedite the property change
            VisualisationController.updateProperties();
            controlsLayer.UpdateControls();
        }


        // The following functions assume a scatterplot, but can be overridden by subclasses for their specific functionality.
        public virtual Vector3 GetVisualisationDimensions()
        {
            return new Vector3(
                VisualisationController.width,
                VisualisationController.height,
                VisualisationController.depth
                );
        }

        protected bool IsAxisDefined(DimensionFilter filter)
        {
            return filter != null && filter.Attribute != "Undefined";
        }

        public virtual void SetVisualisationDimensions(Vector3 dimensions)
        {
            VisualisationController.width = dimensions.x;
            VisualisationController.height = dimensions.y;
            VisualisationController.depth = dimensions.z;

            // Expedite the property change
            MDVMUpdate();
        }

        public virtual bool HasAxis(AxisDirection dir)
        {
            if(Bridge == null || VisualisationController == null)
            {
                // We do not know yet...
                return false;
            }

            return
                (dir == AxisDirection.X && IsAxisDefined(VisualisationController.xDimension)) ||
                (dir == AxisDirection.Y && IsAxisDefined(VisualisationController.yDimension)) ||
                (dir == AxisDirection.Z && IsAxisDefined(VisualisationController.zDimension));
        }

        public virtual DimensionFilter GetAxisFilterIfDefined(AxisDirection dir)
        {
            if (dir == AxisDirection.X && IsAxisDefined(VisualisationController.xDimension))
            {
                return VisualisationController.xDimension;
            }
            if (dir == AxisDirection.Y && IsAxisDefined(VisualisationController.yDimension))
            {
                return VisualisationController.yDimension;
            }
            if (dir == AxisDirection.Z && IsAxisDefined(VisualisationController.zDimension))
            {
                return VisualisationController.zDimension;
            }

            return null;
        }

        /// <summary>
        /// Gets the filter of an axis. I.e. the range of values from min to max that are currently displayed. Always inside the scale.
        /// </summary>
        /// <param name="dir">The axis direction to get the filter from.</param>
        /// <returns>A Vector2 denoting the minimum and the maximum value for the filter of an axis.</returns>
        public virtual Vector2 GetFilter(AxisDirection dir)
        {
            DimensionFilter filter = GetAxisFilterIfDefined(dir);
            if(filter != null)
            {
                return new Vector2(
                    filter.minFilter, filter.maxFilter
                    );
            }
            else
            {
                throw new UnityException("Axis direction " + dir + " is not defined and has no filter.");
            }
        }

        public virtual void SetFilter(AxisDirection dir, Vector2 newFilterValues)
        {
            DimensionFilter filter = GetAxisFilterIfDefined(dir);
            if (filter != null)
            {
                filter.minFilter = newFilterValues.x;
                filter.maxFilter = newFilterValues.y;

                // Expedite the property change
                MDVMUpdate();
            }
            else
            {
                throw new UnityException("Axis direction " + dir + " is not defined and has no filter.");
            }
        }

        /// <summary>
        /// Gets the scale of an axis. I.e. the total range of values from min to max that are displayed on the axis.
        /// </summary>
        /// <param name="dir">The axis direction to get the scale from.</param>
        /// <returns>A Vector2 denoting the minimum and the maximum value for the scale of an axis.</returns>
        public virtual Vector2 GetScale(AxisDirection dir)
        {
            DimensionFilter filter = GetAxisFilterIfDefined(dir);
            if (filter != null)
            {
                return new Vector2(
                    filter.minScale, filter.maxScale
                    );
            }
            else
            {
                throw new UnityException("Axis direction " + dir + " is not defined and has no scale.");
            }
        }

        public virtual void SetScale(AxisDirection dir, Vector2 newFilterValues)
        {
            DimensionFilter filter = GetAxisFilterIfDefined(dir);
            if (filter != null)
            {
                filter.minScale = newFilterValues.x;
                filter.maxScale = newFilterValues.y;

                // Expedite the property change
                MDVMUpdate();
            }
            else
            {
                throw new UnityException("Axis direction " + dir + " is not defined and has no scale.");
            }
        }
    }

}
