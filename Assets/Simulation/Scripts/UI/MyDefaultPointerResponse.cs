using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MDVM.UI
{
    public class MyDefaultPointerResponse : MyPointerResponse
    {
        // Start is called before the first frame update
        void Start()
        {
            // Click for the action menu if it exists.
            MyActionMenuProvider actionMenuProvider = GetComponent<MyActionMenuProvider>();
            if (actionMenuProvider != null) 
            {
                OnClick.AddListener(actionMenuProvider.LaunchActionMenu); 
            }

            // Long click for grabbing if it exists.
            MyGrabbable grabbable = GetComponent<MyGrabbable>();
            if(grabbable != null)
            {
                OnLongClickBegin.AddListener(grabbable.DoGrab);
                OnLongClickBegin.AddListener(() => UISoundSystem.PlayS(UISoundSystem.Get().grabbableGrabBegin));
                OnLongClickEnd.AddListener(grabbable.StopGrab);
                OnLongClickEnd.AddListener(() => UISoundSystem.PlayS(UISoundSystem.Get().grabbableGrabEnd));
            }
        }
    }
}

