using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public float GetRotation()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public bool IsAccelerating()
    {
        if(GameSettings.ControlScheme)
        {
            return Input.GetMouseButton(1) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        }
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
    }

    public bool IsShooting()
    {
        if (GameSettings.ControlScheme)
        {
            return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);
        }
        return Input.GetKeyDown(KeyCode.Space);
    }
}
