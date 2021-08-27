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

