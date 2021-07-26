using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM
{
    public class MyActionMenuActivator : MonoBehaviour
    {
        public GameObject ReferencePoint;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public GameObject CheckActionMenuActivation()
        {
            GameObject objectToActivate = CanActivate();

            MyActionMenuProvider provider = objectToActivate.GetComponent<MyActionMenuProvider>();
            provider.LaunchActionMenu();

            return objectToActivate;
        }

        public void CheckActionMenuActivation(MyPointBehaviour.PointRaycastData data)
        {
            CheckActionMenuActivation();
        }

        // Return the game object currently being activated
        public GameObject CanActivate()
        {
            RaycastHit[] hits = Physics.RaycastAll(ReferencePoint.transform.position, ReferencePoint.transform.forward);

            Collider closest = null;

            foreach (RaycastHit hit in hits)
            {
                Collider collider = hit.collider;
                MyActionMenuProvider colliderProvider = collider.GetComponent<MyActionMenuProvider>();
                if (colliderProvider != null && colliderProvider.enabled)
                {
                    if (closest == null)
                    {
                        closest = collider;
                    }
                    else
                    {
                        // Position of ourselves and not the reference.
                        Vector3 ourPosition = transform.position;
                        if (
                            Vector3.Distance(closest.transform.position, ourPosition) >=
                            Vector3.Distance(collider.transform.position, ourPosition)
                            )
                        {
                            closest = collider;
                        }
                    }
                }
            }

            GameObject objToGrab = closest != null ? closest.gameObject : null;

            return objToGrab;
        }
    }

}