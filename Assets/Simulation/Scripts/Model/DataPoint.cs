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
using UnityEngine.EventSystems;

using MDVM.UI;

namespace MDVM.Model
{
    public class DataPoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected bool selected = false;

        internal protected Vector3 dataPointReference = Vector3.zero;
        internal protected long uniqueCreationID = 0;

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                ChangeMaterialOnSelect();
            }
        }

        public Material unselectedMaterial = null;
        public Material selectedMaterial = null;
        public Material hoverMaterial = null;
        public Material selectedHoverMaterial = null;

        public AudioClip soundOnSelect = null;
        public AudioClip soundOnDeselect = null;

        MeshRenderer ourRenderer = null;

        // Start is called before the first frame update
        void Start()
        {
            ourRenderer = GetComponentInChildren<MeshRenderer>();

            if (ourRenderer == null)
            {
                ourRenderer = GetComponent<MeshRenderer>();
            }

            ChangeMaterialOnSelect();
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected void ChangeMaterialOnSelect()
        {
            if(Selected && selectedMaterial != null)
            {
                ourRenderer.sharedMaterial = selectedMaterial;
            }
            else
            {
                ourRenderer.sharedMaterial = unselectedMaterial;
            }

        }

        

        public void Select()
        {
            Selected = true;
        }

        public void Deselect()
        {
            Selected = false;
        }

        public void ToggleSelect()
        {
            Selected = !Selected;
        }

        public void ActionSelect()
        {
            ToggleSelect(); 
            PlayAssocSound();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(Selected && selectedHoverMaterial != null)
            {
                ourRenderer.sharedMaterial = selectedHoverMaterial;
            }
            else if(!Selected && hoverMaterial != null)
            {
                ourRenderer.sharedMaterial = hoverMaterial;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ChangeMaterialOnSelect();
        }

        protected void PlayAssocSound()
        {
            if(Selected)
            {
                UISoundSystem.PlayS(soundOnSelect);
            }
            else
            {
                UISoundSystem.PlayS(soundOnDeselect);
            }
        }
    }
}