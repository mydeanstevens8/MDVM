using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MDVM.UI
{
    public class ButtonDelay : MonoBehaviour
    {
        Image buttonImage = null;
        GameObject child = null;

        public float popupDelay = 0.0f;
        public AudioClip popupSound = null;

        public float popupAnimationTime = 0.05f;

        public GameObject backButtonDisplay = null;
        public GameObject forwardButtonDisplay = null;

        float timeAtStart = 0.0f;
        bool donePopup = false;
        bool completedPopupAnimation = false;

        // Start is called before the first frame update
        void Start()
        {
            buttonImage = GetComponent<Image>();
            child = transform.GetChild(0).gameObject;

            timeAtStart = Time.time;

            if (popupDelay > 0.0f)
            {
                buttonImage.enabled = false;
                child.SetActive(false);
            }
            else
            {
                donePopup = true;
                completedPopupAnimation = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            float startAnimationTime = timeAtStart + popupDelay;
            float timeSinceBegin = Time.time - startAnimationTime;

            // I should use coroutines here... but okay.
            if (timeSinceBegin >= 0 && !completedPopupAnimation)
            {
                // Initial enable
                if (!donePopup)
                {
                    buttonImage.enabled = true;
                    child.SetActive(true);

                    if (popupSound != null)
                    {
                        AudioSource playSoundAt = GetComponent<AudioSource>();
                        playSoundAt.PlayOneShot(popupSound);
                    }

                    donePopup = true;
                }

                float animationFraction = Mathf.Clamp01(timeSinceBegin / popupAnimationTime);

                if (animationFraction == 1.0f)
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
            }
        }
    }
}
