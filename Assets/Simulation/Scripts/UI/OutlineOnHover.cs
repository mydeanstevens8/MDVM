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