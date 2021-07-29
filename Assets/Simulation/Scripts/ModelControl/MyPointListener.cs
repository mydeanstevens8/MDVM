using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM
{
    public class MyPointListener : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private bool isHovering = false;

        public virtual void OnPointerHoverEnter(GameObject pointerObj)
        {
            Debug.Log("Hovering over an object.", this);
        }

        public virtual void OnPointerHoverExit(GameObject pointerObj)
        {
            Debug.Log("Exiting hover over an object.", this);
        }

        internal void RegisterHovering(bool hovering)
        {
            isHovering = hovering;
        }

        internal bool CheckHovering()
        {
            return isHovering;
        }
    }

}
