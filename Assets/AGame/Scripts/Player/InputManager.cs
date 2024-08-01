using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDefaultKeyBindings();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void InitializeDefaultKeyBindings()
    {
        // Setup default hotkey
        keyBindings["MoveForward"] = KeyCode.W;
        keyBindings["MoveBackward"] = KeyCode.S;
        keyBindings["MoveLeft"] = KeyCode.A;
        keyBindings["MoveRight"] = KeyCode.D;
        keyBindings["Run"] = KeyCode.LeftShift;
        keyBindings["ResetCamPos"] = KeyCode.R;
        keyBindings["MoveToMouse"] = KeyCode.Mouse0;
        keyBindings["RotateCamera"] = KeyCode.Mouse1;
        keyBindings["ZoomCamera"] = KeyCode.Mouse2;
        // Add new hotkey in here
    }

    // Check hotkey click down
    public bool GetKeyDown(string actionName)
    {
        if (keyBindings.TryGetValue(actionName, out KeyCode keyCode))
        {
            return Input.GetKeyDown(keyCode);
        }
        Debug.LogWarning($"Action {actionName} not found in key bindings.");
        return false;
    }

    // Check hotkey holding
    public bool GetKey(string actionName)
    {
        if (keyBindings.TryGetValue(actionName, out KeyCode keyCode))
        {
            return Input.GetKey(keyCode);
        }
        Debug.LogWarning($"Action {actionName} not found in key bindings.");
        return false;
    }

    public bool IsMoving()
    {
        return GetKey("MoveForward") || GetKey("MoveBackward") || GetKey("MoveLeft") || GetKey("MoveRight");
    }

    public bool IsRunning()
    {
        return GetKey("Run");
    }

    public bool IsMoveToMouse()
    {
        return GetKey("MoveToMouse");
    }

    public bool IsCamRotate()
    {
        return GetKey("RotateCamera");
    }

    public bool IsCamZoom()
    {
        return GetKey("ZoomCamera");
    }

    public bool IsResetCamPos()
    {
        return GetKey("ResetCamPos");
    }
}
