using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorDebug : MonoBehaviour
{
    public float MotionSpeed = 10.0f;
    public float RotationSpeed = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // WASD input
        MoveCharacterByInput();
    }

    void MoveCharacterByInput()
    {
        float xAxis = Input.GetAxis("Editor_Debug_X") * RotationSpeed;
        float zAxis = Input.GetAxis("Editor_Debug_Z") * MotionSpeed;
        float yAxis = Input.GetAxis("Editor_Debug_Y") * MotionSpeed;

        CharacterController ourController = GetComponent<CharacterController>();

        ourController.Move((transform.up * yAxis + transform.forward * zAxis) * Time.fixedDeltaTime);
        transform.Rotate(new Vector3(0, xAxis, 0) * Time.fixedDeltaTime);
    }
}
