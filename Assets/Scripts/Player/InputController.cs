using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class responsible for input reading
public class InputController : MonoBehaviour
{
    //Gets input for calculating rotation for keyboard control scheme
    public float GetRotation()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    //Gets input for accelerating ship for both control schemes
    public bool IsAccelerating()
    {
        if(GameSettings.instance.ControlScheme)
        {
            return Input.GetMouseButton(1) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        }
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
    }

    //Gets input for shooting for both control schemes
    public bool IsShooting()
    {
        if (GameSettings.instance.ControlScheme)
        {
            return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);
        }
        return Input.GetKeyDown(KeyCode.Space);
    }
}
