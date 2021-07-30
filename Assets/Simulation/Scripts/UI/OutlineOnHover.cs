using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace MDVM.UI
{
    public class OutlineOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public MeshRenderer rendererToInject = null;
        public Material outlineMaterial = null;

        public float outlineWidth = 0.01f;

        public Color inactiveColor = new Color(0, 0, 0, 0);
        public Color outlineColor = new Color(0, 1, 1, 0.5f);

        private Material instancedOutlineMaterial = null;

        MeshRenderer render = null;

        private void Start()
        {
            render = rendererToInject != null ? rendererToInject : GetComponent<MeshRenderer>();

            // Create a copy of the material to use inside
            instancedOutlineMaterial = Instantiate(outlineMaterial);

            // Inject the material into the object for rendering on demand.
            if (render != null && instancedOutlineMaterial != null)
            {
                Material[] materialsList = render.materials;
                Array.Resize(ref materialsList, materialsList.Length + 1);

                // Set the last index to our outline material.
                materialsList[materialsList.Length - 1] = instancedOutlineMaterial;
                render.materials = materialsList;

                // Configure the material color to transparent for now.
                instancedOutlineMaterial.SetColor("_OutlineColor", inactiveColor);
                instancedOutlineMaterial.SetFloat("_OutlineWidth", outlineWidth);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Outline applied to object.");
            // Configure the material color
            instancedOutlineMaterial.SetColor("_OutlineColor", outlineColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Configure the material color
            instancedOutlineMaterial.SetColor("_OutlineColor", inactiveColor);
        }
    }

}