using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody2D cameraTarget;
    public float cameraMoveSpeed = 1f;
    private InputManager inputs;

    private Vector2 moveDir = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        inputs = InputManager.Instance;
        inputs.Input.GameInputs.MoveCamera.performed += OnMoveKeyPressed;
        inputs.Input.GameInputs.MoveCamera.canceled += OnMoveKeyReleased;

    }

    private void OnMoveKeyReleased(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        moveDir = Vector2.zero;
    }

    private void OnMoveKeyPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        cameraTarget.MovePosition(cameraTarget.position + moveDir * cameraMoveSpeed);
    }
}
