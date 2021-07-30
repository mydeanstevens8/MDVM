using System.Collections;
using System.Collections.Generic;
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
            Debug.Log("Long clicking from pointer.", this);
            isLongClicking = true;
            OnLongClickBegin.Invoke();

            yield return new WaitForEndOfFrame();
        }
    }
}

