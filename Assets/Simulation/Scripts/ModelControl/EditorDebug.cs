using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorDebug : MonoBehaviour
{
    public float MotionSpeed = 10.0f;
    public float RotationSpeed = 100.0f;

    public GameObject cameraRig = null;

    // Start is called before the first frame update
    void Start()
    {
        if(!Application.isEditor)
        {
            enabled = false;
        }
        else
        {
            // Lock the cursor.
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!Input.GetKey(KeyCode.Z)) {
            RotateCharacterByInput();
        }
    }

    private void FixedUpdate()
    {
        // WASD input
        MoveCharacterByInput();
    }

    void MoveCharacterByInput()
    {
        float xAxis = Input.GetAxis("Editor_Debug_X") * MotionSpeed;
        float zAxis = Input.GetAxis("Editor_Debug_Z") * MotionSpeed;
        float yAxis = Input.GetAxis("Editor_Debug_Y") * MotionSpeed;

        CharacterController ourController = GetComponent<CharacterController>();

        ourController.Move((transform.right * xAxis + transform.up * yAxis + transform.forward * zAxis) * Time.fixedDeltaTime);
    }

    void RotateCharacterByInput()
    {
        float xAxis = Input.GetAxis("Editor_Debug_RotX") * RotationSpeed;
        float yAxis = Input.GetAxis("Editor_Debug_RotY") * RotationSpeed;

        transform.Rotate(0, xAxis, 0);

        if(cameraRig != null)
        {
            Transform crtrans = cameraRig.transform;
            crtrans.Rotate(-yAxis, 0, 0);
        }
    }
}
