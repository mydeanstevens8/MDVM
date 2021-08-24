using System.Collections;
using System.Collections.Generic;
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