using System;
using System.Collections.Generic;

using UnityEngine;

namespace MDVM.Gesture
{
    interface IGestureEndable
    {
        // Subclass responsible for checking whether to end or not based on state.
        public void OnGestureEnd();
    }

    class GestureEnding : Gesture
    {
        public IGestureEndable gestureEndable;
        public Gesture injectGestureEndable = null;

        public void Start()
        {
            GestureInit();

            if(injectGestureEndable && injectGestureEndable is IGestureEndable)
            {
                gestureEndable = injectGestureEndable as IGestureEndable;
            }
        }

        protected override void OnHandGesture(OVRHand hand)
        {
            // On the specific hand gesture activated.
            gestureEndable.OnGestureEnd();
        }

        protected override void OnHandGestureEnd(OVRHand hand)
        {
            // On the specific hand gesture activated.
            
        }
    }
}
