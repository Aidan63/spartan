using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;

public class BipedController : MonoBehaviour
{
    public BipedMotor motor;

    public void OnJump()
    {
        motor.Jump();
    }

    public void OnMove(InputValue value)
    {
        motor.Input(value.Get<float>());
    }
}
