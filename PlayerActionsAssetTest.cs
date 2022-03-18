using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionsAssetTest : MonoBehaviour
{
    MyInputSystem inputActions;

    public Vector2 move;
    public Vector2 look;
    public bool isHoldDash;

    InputAction jumpAction;

    public enum StateMove
    {
        Run,
        Walk,
        SpeedRun
    }
    public StateMove stateMove;

    private void Awake()
    {
        inputActions = new MyInputSystem();
    }

    private void OnEnable()
    {
        inputActions.Player.Move.performed += OnMovement;
        inputActions.Player.Move.canceled += ClearMovement;

        inputActions.Player.DashStart.performed += OnDashStart;
        inputActions.Player.DashEnd.performed += OnDashEnd;

        inputActions.Player.ChangeStateMove.performed += ChangeStateMove;

        jumpAction = inputActions.Player.Jump;

        inputActions.Player.Enable();
    }

    private void Update()
    {
        look = inputActions.Player.Look.ReadValue<Vector2>();
    }

    public bool OnJump()
    {
        return jumpAction.triggered;
    }

    private void OnDashEnd(InputAction.CallbackContext obj)
    {
        isHoldDash = false;
    }

    private void OnDashStart(InputAction.CallbackContext obj)
    {
        isHoldDash = true;
    }
    
    private void ClearMovement(InputAction.CallbackContext obj)
    {
        move = Vector2.zero;
    }

    private void OnMovement(InputAction.CallbackContext obj)
    {
        move = obj.ReadValue<Vector2>();
    }

    private void ChangeStateMove(InputAction.CallbackContext obj)
    {
        if (stateMove == StateMove.Run)
        {
            stateMove = StateMove.Walk;
        }
        else if (stateMove == StateMove.Walk)
        {
            stateMove = StateMove.Run;
        }
    }

    public void SetCursorLock(bool isShow)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = isShow;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
}
