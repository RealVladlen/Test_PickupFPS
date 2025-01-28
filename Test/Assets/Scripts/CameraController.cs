using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerBody; 
    [SerializeField] private Transform playerHead; 
    [SerializeField] private float sensitivity = 1f; 
    [SerializeField] private float verticalClamp = 80f; 

    private float _verticalRotation = 0f; 
    private Vector2 _lastTouchPosition; 
    private bool _isSwiping;

    private void Update()
    {
        if (IsPointerDown() && !IsPointerOverFrameNotTouch())
        {
            if (IsPointerOverFrameTouchTarget())
            {
                _isSwiping = true;
                _lastTouchPosition = GetPointerPosition();
            }
        }

        if (_isSwiping && IsPointerHeld())
        {
            Vector2 currentTouchPosition = GetPointerPosition();
            Vector2 delta = currentTouchPosition - _lastTouchPosition;

            RotateCamera(delta);

            _lastTouchPosition = currentTouchPosition;
        }

        if (IsPointerUp())
            _isSwiping = false;
    }

    private void RotateCamera(Vector2 delta)
    {
        float horizontalRotation = delta.x * sensitivity;
        playerBody.Rotate(Vector3.up, horizontalRotation);

        float verticalRotation = -delta.y * sensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation + verticalRotation, -verticalClamp, verticalClamp);
        playerHead.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
    }

    private Vector2 GetPointerPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;

        return Input.mousePosition;
    }

    private bool IsPointerOverFrameNotTouch()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = GetPointerPosition()
        };

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<FrameNotTouch>() != null)
                return true; 
        }

        return false;
    }

    private bool IsPointerOverFrameTouchTarget()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = GetPointerPosition()
        };

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<FrameTouchTarget>() != null)
                return true;
        }

        return false;
    }

    private bool IsPointerDown()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Began;

        return Input.GetMouseButtonDown(0);
    }

    private bool IsPointerHeld()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary;

        return Input.GetMouseButton(0);
    }

    private bool IsPointerUp()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Ended;

        return Input.GetMouseButtonUp(0);
    }
}