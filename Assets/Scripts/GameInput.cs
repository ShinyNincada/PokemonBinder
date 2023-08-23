using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnBackwardAction;
    public static GameInput Instance { get; private set; }
    public PlayerInputAction inputActions;
    private void Awake() {
        if(Instance == null){
            Instance = this;
        }
        else{
            Debug.LogWarning("Another instance exsited!");
        }
        inputActions = new PlayerInputAction();
        inputActions.Player.Enable();
        inputActions.Player.Backward.performed += Backward_performed;
    }

    private void Backward_performed(InputAction.CallbackContext context)
    {
        OnBackwardAction?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy() {
        if(inputActions != null){
            inputActions.Player.Backward.performed -= Backward_performed;
            inputActions.Dispose();
        }
    }
}
