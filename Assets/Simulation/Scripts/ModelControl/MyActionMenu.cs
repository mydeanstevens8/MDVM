namespace MDVM {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MyActionMenu : MonoBehaviour
    {
        Canvas myCanvas = null;

        GameObject debugImage = null;
        

        // Start is called before the first frame update
        void Start()
        {
            myCanvas = GetComponent<Canvas>();
            debugImage = GameObject.Find("CanvasDebugImage");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ActivateActionMenu()
        {
            // Initially deactivate
            DeactivateActionMenu();

            Camera referenceCamera = myCanvas.worldCamera;

            transform.LookAt(referenceCamera.transform.position, Vector3.up);
            // Make the forward vector point away from the camera.
            transform.rotation = Quaternion.Inverse(transform.rotation);

            // Action menu debug
            if(debugImage != null)
            {
                debugImage.SetActive(true);
            }
        }

        public void DeactivateActionMenu()
        {
            if(debugImage != null)
            {
                debugImage.SetActive(false);
            }
        }
    }
}

