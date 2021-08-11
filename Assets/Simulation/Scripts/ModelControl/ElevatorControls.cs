using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM
{
    public class ElevatorControls : MonoBehaviour
    {
        public GameObject reference = null;

        public float elevateStrength = 1.0f;
        public float elevatorTimeScale = 1.0f;

        public float elevatorMinSpeed = 0.1f;
        public float elevatorMaxSpeed = 10.0f;

        Vector3 elevatorDirection = Vector3.up;
        float elevatorStartTime = 0.0f;
        bool activeElevator = false;

        public void ActivateElevator(Vector3 direction)
        {
            UI.UISoundSystem.PlayS(UI.UISoundSystem.Get().elevatorBegin);

            elevatorDirection = direction.normalized;
            elevatorStartTime = Time.time;
            activeElevator = true;
        }

        public void ActivateElevatorUp()
        {
            UI.UISoundSystem.PlayS(UI.UISoundSystem.Get().elevatorUp);
            ActivateElevator(Vector3.up);
        }
        public void ActivateElevatorDown()
        {
            UI.UISoundSystem.PlayS(UI.UISoundSystem.Get().elevatorDown);
            ActivateElevator(Vector3.down);
        }

        public void DeactivateElevator()
        {
            UI.UISoundSystem.PlayS(UI.UISoundSystem.Get().elevatorComplete);
            activeElevator = false;
        }

        // On fixed update due to the potential use of physics.
        private void FixedUpdate()
        {
            if(activeElevator)
            {
                float timeElapsed = Time.time - elevatorStartTime;

                float movementSpeed =
                    Mathf.Clamp(elevateStrength * Mathf.Pow(2, elevatorTimeScale * timeElapsed), elevatorMinSpeed, elevatorMaxSpeed);

                Vector3 scaledDirection = elevatorDirection * movementSpeed;

                if(reference != null)
                {
                    CharacterController control = reference.GetComponent<CharacterController>();

                    if(control != null)
                    {
                        control.Move(Time.fixedDeltaTime * scaledDirection);
                    }
                    else
                    {
                        reference.transform.position += Time.fixedDeltaTime * scaledDirection;
                    }
                }
            }
        }
    }
}

