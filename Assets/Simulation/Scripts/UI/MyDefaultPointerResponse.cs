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

