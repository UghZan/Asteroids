using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    [SerializeField] Renderer VisualRenderer;
    Camera _camera;
    Vector2 _viewPos;

    //control flags used to prevent object infinitely bouncing around bounds of screen
    //makes it so translation from screen bound to screen bound happens once
    bool _wrappedXOnce, _wrappedYOnce;

    void Start()
    {
        if (VisualRenderer == null) VisualRenderer = GetComponent<Renderer>();
        _camera = Camera.main;
    }

    void Update()
    {
        Wrap();
    }

    //uses renderer to decide if object is visible (inside viewport)
    bool CheckVisibility()
    {
        _viewPos = _camera.WorldToViewportPoint(transform.position);
        /*if (_viewPos.x > GameSettings.minScreenWrapBorder && _viewPos.x < GameSettings.maxScreenWrapBorder 
            && _viewPos.y > GameSettings.minScreenWrapBorder && _viewPos.y < GameSettings.maxScreenWrapBorder) return true;
        return false;*/
        return VisualRenderer.isVisible;
    }

    void Wrap()
    {
        if (CheckVisibility())
        {
            _wrappedXOnce = false;
            _wrappedYOnce = false;
            return;
        }

        Vector3 newPos = transform.position;
        if (!_wrappedXOnce)
            if (_viewPos.x > GameSettings.MaxScreenWrapBorder || _viewPos.x < GameSettings.MinScreenWrapBorder)
            {
                newPos.x = -newPos.x;
                _wrappedXOnce = true;
            }

        if (!_wrappedYOnce)
            if (_viewPos.y > GameSettings.MaxScreenWrapBorder || _viewPos.y < GameSettings.MinScreenWrapBorder)
            {
                newPos.y = -newPos.y;
                _wrappedYOnce = true;
            }

        //set z to 0 just in case
        newPos.z = 0;
        transform.position = newPos;
    }
}
