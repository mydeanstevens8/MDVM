using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace MDVM.UI
{
    public sealed class ModalDisplay : MonoBehaviour
    {
        public GameObject screenHide = null;
        public GameObject canvasObject = null;
        public TextMeshProUGUI guiTitle = null;

        public Image modalWarningImage = null;

        public Color nonModalColor = Color.white;
        public Color modalWarningColor = Color.blue;

        public int modalWarningRepeats = 4;
        public float modalWarningInterval = 0.2f;

        public GameObject canvasObjectInsertPoint = null;

        public Camera eventCamera = null;

        [SerializeField]
        AudioClip activateModalDisplayAudio = null;
        [SerializeField]
        AudioClip deactivateModalDisplayAudio = null;

        [SerializeField]
        AudioClip modalDisplayWarning = null;

        [SerializeField]
        bool playAudio = true;

        // It is important that the audio focus the user here.
        public AudioSource directedAudio = null;

        // Singleton object.
        static ModalDisplay instance = null;

        private bool showModal = false;

        public bool ModalShowing
        {
            get
            {
                return showModal;
            }
            set
            {
                showModal = value;
                OnModalShow(value);
            }
        }

        private bool instanceRequiresFocus = false;

        void Awake()
        {
            instance = this;

            if (canvasObjectInsertPoint == null)
            {
                canvasObjectInsertPoint = canvasObject;
            }

            OnModalShow(showModal);
            SetModalWarningAnimation(false);
        }

        private void Update()
        {
            if(eventCamera != null && ModalShowing)
            {
                transform.position = eventCamera.transform.position;
            }
        }

        void InitialPositionModal()
        {
            if (eventCamera != null)
            {
                transform.position = eventCamera.transform.position;

                Vector3 rotEu = eventCamera.transform.rotation.eulerAngles;

                // Change only the Y component to center the dialog on the middle.
                transform.rotation = Quaternion.Euler(0, rotEu.y, 0);
            }
        }

        void OnModalShow(bool show)
        {
            if(screenHide)
            {
                screenHide.SetActive(show);
            }
            if(canvasObject)
            {
                canvasObject.SetActive(show);
            }

            if(show)
            {
                InitialPositionModal();
            }
        }

        public void ShowModal(GameObject childPrefab, string title = "Dialog", bool requiresFocus = false)
        {
            ModalShowing = true;
            instanceRequiresFocus = requiresFocus;

            if (canvasObjectInsertPoint != null)
            {
                GameObject newObj = Instantiate(childPrefab, canvasObjectInsertPoint.transform);

                RectTransform rt = newObj.transform as RectTransform;

                // Set up the parent to cover the whole screen.
                if(rt != null)
                {
                    // Set to stretch both axes
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;

                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.localRotation = Quaternion.identity;
                    rt.localScale = Vector3.one;
                    rt.localPosition = Vector3.zero;

                    rt.anchoredPosition = Vector3.zero;
                    rt.sizeDelta = Vector3.one;
                    rt.offsetMax = Vector3.zero;
                    rt.offsetMin = Vector3.zero;


                }

                // Set the title
                if(guiTitle != null)
                {
                    guiTitle.text = title;
                }

                // Link all "Dialog Close Button" scripts to our close routine.
                foreach(DialogCloseButton closeBtn in newObj.GetComponentsInChildren<DialogCloseButton>())
                {
                    Button btn = closeBtn.GetComponent<Button>();
                    btn.onClick.AddListener(ClearModal);
                }
            }

            if(activateModalDisplayAudio != null && playAudio)
            {
                UISoundSystem.PlayS(activateModalDisplayAudio);
            }
        }

        public void ClearModal()
        {
            ModalShowing = false;

            if (canvasObjectInsertPoint != null)
            {
                foreach(Transform child in canvasObjectInsertPoint.transform)
                {
                    // Do so on the destroy loop to be removed on the frame after.
                    Destroy(child.gameObject);
                }
            }

            if (deactivateModalDisplayAudio != null && playAudio)
            {
                UISoundSystem.PlayS(deactivateModalDisplayAudio);
            }
        }

        public void ClearModalUnfocused()
        {
            if(!instanceRequiresFocus)
            {
                ClearModal();
            }
            else
            {
                // Warn the user for modality.
                if (modalDisplayWarning != null && playAudio)
                {
                    if(directedAudio != null)
                    {
                        directedAudio.PlayOneShot(modalDisplayWarning);
                    }
                    else
                    {
                        UISoundSystem.PlayS(modalDisplayWarning);
                    }
                }

                StartCoroutine(DoModalWarningAnimation());
            }
        }

        void SetModalWarningAnimation(bool modalWarningFlag)
        {
            if(modalWarningImage)
            {
                modalWarningImage.color = modalWarningFlag ? modalWarningColor : nonModalColor;
            }
        }

        IEnumerator DoModalWarningAnimation()
        {
            for(int i = 0; i < modalWarningRepeats; i++)
            {
                SetModalWarningAnimation(true);
                yield return new WaitForSecondsRealtime(modalWarningInterval);
                SetModalWarningAnimation(false);
                yield return new WaitForSecondsRealtime(modalWarningInterval);
            }
            SetModalWarningAnimation(false);
            yield return null;
        }

        public static ModalDisplay Instance
        {
            get
            {
                return instance;
            }
        }
    }
}

