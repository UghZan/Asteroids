using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that adds screen wrapping capability to object it is put on
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

    //Uses renderer to decide if object is visible (inside viewport)
    bool CheckVisibility()
    {
        _viewPos = _camera.WorldToViewportPoint(transform.position);
        
        return VisualRenderer.isVisible;
    }

    bool ViewportPositionOutOfBounds()
    {
        return (_viewPos.x < 0 && _viewPos.x > 1
            && _viewPos.y > 1 && _viewPos.y < 0);
    }

    //Wraps object position around screen
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
            if (_viewPos.x < 0 || _viewPos.x > 1)
            {
                newPos.x = -newPos.x;
                _wrappedXOnce = true;
            }

        if (!_wrappedYOnce)
            if (_viewPos.y < 0 || _viewPos.y > 1)
            {
                newPos.y = -newPos.y;
                _wrappedYOnce = true;
            }

        //sets z to 0 just in case
        newPos.z = 0;
        transform.position = newPos;

        //if object by any chance flies out of bounds and doesn't come back (very rarely that happens for some reason), forcefully bring it back
        if (_wrappedXOnce && _wrappedYOnce && ViewportPositionOutOfBounds())
        {
            MoveInBounds();
        }
    }

    void MoveInBounds()
    {
        Vector2 newViewPos = new Vector2(Mathf.Clamp01(_viewPos.x), Mathf.Clamp01(_viewPos.y));
        transform.position = _camera.ViewportToWorldPoint(newViewPos);
    }
}
