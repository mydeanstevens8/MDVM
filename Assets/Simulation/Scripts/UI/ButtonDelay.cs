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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MDVM.UI
{
    public class ButtonDelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        Image buttonImage = null;

        public float popupDelay = 0.0f;
        public AudioClip popupSound = null;

        public float popupAnimationTime = 0.05f;
        public float hoverAnimationTime = 0.1f;

        public float hoverExtraScaling = 0.1f;

        public TextMeshProUGUI child = null;
        public GameObject backButtonDisplay = null;
        public GameObject forwardButtonDisplay = null;

        protected Coroutine activeAnimationCoroutine = null;
        protected Queue<IEnumerator> animationQueue = new Queue<IEnumerator>();

        public Color passiveTextColor = new Color(0.0f, 0.0f, 0.0f);
        public Color activeTextColor = new Color(0.0f, 0.75f, 0.0f);


        // Start is called before the first frame update
        void Start()
        {
            buttonImage = GetComponent<Image>();
            child = transform.GetComponentInChildren<TextMeshProUGUI>();

            if (popupDelay > 0.0f)
            {
                // Initial setup
                SetVisibleButton(false);
                SetAnimationCoroutine(PopupAnimationCoroutine());
            }

            child.color = passiveTextColor;
        }

        public void SetAnimationCoroutine(IEnumerator newCoroutine)
        {
            animationQueue.Enqueue(newCoroutine);
        }

        public void SetVisibleButton(bool visible)
        {
            buttonImage.enabled = visible;
            child.gameObject.SetActive(visible);
            transform.localScale = visible ? Vector3.one : Vector3.zero;
        }

        // Animation coroutines here
        IEnumerator PopupAnimationCoroutine()
        {
            yield return new WaitForSeconds(popupDelay);


            float animationFraction;
            float timeSinceBegin;

            bool completedPopupAnimation = false;
            float startTime = Time.time;

            SetVisibleButton(true);

            if (popupSound != null)
            {
                AudioSource playSoundAt = GetComponent<AudioSource>();
                playSoundAt.PlayOneShot(popupSound);
            }

            while (!completedPopupAnimation)
            {
                timeSinceBegin = Time.time - startTime;
                animationFraction = Mathf.Clamp01(timeSinceBegin / popupAnimationTime);

                // Allow the animation fraction 1 pass to run
                if(animationFraction == 1.0f)
                {
                    completedPopupAnimation = true;
                }

                Color buttonLastColor = buttonImage.color;

                // Fade in
                Color newColor = new Color(buttonLastColor.r, buttonLastColor.g, buttonLastColor.b, animationFraction);
                buttonImage.color = newColor;

                // Custom scaling.
                float transformedAnimationFraction = -animationFraction * (Mathf.Pow(animationFraction, 2.0f) - 2);

                // Scale in
                transform.localScale = new Vector3(transformedAnimationFraction, transformedAnimationFraction, transformedAnimationFraction);

                yield return null;
            }

            activeAnimationCoroutine = null;
            yield return null;
        }


        IEnumerator ButtonHoverCoroutine()
        {
            yield return null;
            child.color = activeTextColor;

            float animationFraction;
            float timeSinceBegin;

            bool completedPopupAnimation = false;
            float startTime = Time.time;

            while (!completedPopupAnimation)
            {
                timeSinceBegin = Time.time - startTime;
                animationFraction = Mathf.Clamp01(timeSinceBegin / hoverAnimationTime);

                // Allow the animation fraction 1 pass to run
                if (animationFraction == 1.0f)
                {
                    completedPopupAnimation = true;
                }

                // Custom scaling.
                float transformedAnimationFraction = 1.0f + hoverExtraScaling * animationFraction;

                // Scale in
                transform.localScale = new Vector3(transformedAnimationFraction, transformedAnimationFraction, transformedAnimationFraction);

                yield return null;
            }

            transform.localScale = Vector3.one + hoverExtraScaling * Vector3.one;
            activeAnimationCoroutine = null;
            yield return null;
        }

        IEnumerator ButtonUnhoverCoroutine()
        {
            yield return null;
            child.color = passiveTextColor;

            float animationFraction;
            float timeSinceBegin;

            bool completedPopupAnimation = false;
            float startTime = Time.time;

            while (!completedPopupAnimation)
            {
                timeSinceBegin = Time.time - startTime;
                animationFraction = Mathf.Clamp01(timeSinceBegin / hoverAnimationTime);

                // Allow the animation fraction 1 pass to run
                if (animationFraction == 1.0f)
                {
                    completedPopupAnimation = true;
                }

                // Custom scaling.
                float transformedAnimationFraction = 1.0f + (1 - animationFraction) * hoverExtraScaling;

                // Scale in
                transform.localScale = new Vector3(transformedAnimationFraction, transformedAnimationFraction, transformedAnimationFraction);

                yield return null;
            }

            transform.localScale = Vector3.one;
            activeAnimationCoroutine = null;
            yield return null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetAnimationCoroutine(ButtonHoverCoroutine());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetAnimationCoroutine(ButtonUnhoverCoroutine());
        }

        private void Update()
        {
            if (activeAnimationCoroutine == null && animationQueue.Count > 0)
            {
                activeAnimationCoroutine = StartCoroutine(animationQueue.Dequeue());
            }
        }
    }
}
