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

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MDVM.UI
{
    public class MyPointerResponse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public UnityEvent OnClick = new UnityEvent();
        public UnityEvent OnPressDown = new UnityEvent();
        public UnityEvent OnPressUp = new UnityEvent();

        public UnityEvent OnLongClickBegin = new UnityEvent();
        public UnityEvent OnLongClickEnd = new UnityEvent();
        public UnityEvent OnLongClick = new UnityEvent();

        public UnityEvent OnHover = new UnityEvent();
        public UnityEvent OnHoverEnd = new UnityEvent();

        public float LongClickDelay = 0.3f;

        private Coroutine LongClickCoroutine;
        bool isLongClicking = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isLongClicking)
            {
                OnLongClick.Invoke();
            }
            else
            {
                OnClick.Invoke();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isLongClicking = false;
            OnPressDown.Invoke();
            LongClickCoroutine = StartCoroutine(LongClickFunction());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPressUp.Invoke();
            if(isLongClicking)
            {
                OnLongClickEnd.Invoke();
            }

            StopCoroutine(LongClickCoroutine);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnHoverEnd.Invoke();
        }

        IEnumerator LongClickFunction()
        {
            isLongClicking = false;
            yield return new WaitForSecondsRealtime(LongClickDelay);
            // Debug.Log("Long clicking from pointer.", this);
            isLongClicking = true;
            OnLongClickBegin.Invoke();

            yield return new WaitForEndOfFrame();
        }
    }
}

