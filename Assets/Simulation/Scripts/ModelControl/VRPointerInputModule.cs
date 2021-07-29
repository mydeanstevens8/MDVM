using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MDVM
{
    public class VRPointerInputModule : StandaloneInputModule
    {
        public GameObject PointerObjectL = null;
        public GameObject PointerObjectR = null;

        private GraphicRaycaster[] m_GraphicRaycasters = null;

        protected override void Start()
        {
            base.Start();
            m_GraphicRaycasters = FindObjectsOfType<GraphicRaycaster>();
        }

        public override void Process()
        {
            // Call super's implementation
            base.Process();
        }

        Ray GetRayFromPointer(GameObject pointer)
        {
            return new Ray(pointer.transform.position, pointer.transform.forward);
        }



        public List<GameObject> GetHitOnGraphicRaycaster()
        {
            List<GameObject> objectsHit = new List<GameObject>();

            // Each raycaster gets a hit test on both of our point objects.
            foreach (GraphicRaycaster raycaster in m_GraphicRaycasters) {

                PointerEventData evData = new PointerEventData(eventSystem);

                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(evData, results);

                foreach(RaycastResult result in results)
                {
                    objectsHit.Add(result.gameObject);
                }
            }

            return objectsHit;
        }
    }
}

