using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerBody; 
    [SerializeField] private Transform playerHead; 
    [SerializeField] private float sensitivity = 1f; 
    [SerializeField] private float verticalClamp = 80f; 
    [SerializeField] private float smoothTime = 0.1f; 

    private float _verticalRotation = 0f; 
    private Vector2 _currentDelta;
    private Vector2 _smoothDeltaVelocity; 
    private int _cameraTouchId = -1; 

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (_cameraTouchId == -1 && touch.phase == TouchPhase.Began && IsPointerInRightHalf(touch.position) && !IsPointerOverUI(touch.position))
                {
                    _cameraTouchId = touch.fingerId;
                }

                if (touch.fingerId == _cameraTouchId)
                {
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Vector2 rawDelta = touch.deltaPosition;

                        if (rawDelta.magnitude > 0f)
                        {
                            _currentDelta = Vector2.SmoothDamp(_currentDelta, rawDelta, ref _smoothDeltaVelocity, smoothTime);
                            RotateCamera(_currentDelta);
                        }
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        _cameraTouchId = -1;
                        _currentDelta = Vector2.zero;
                    }
                }
            }
        }
        else if (Input.GetMouseButton(0) && IsPointerInRightHalf(Input.mousePosition) && !IsPointerOverUI(Input.mousePosition))
        {
            Vector2 rawDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            if (rawDelta.magnitude > 0f)
            {
                _currentDelta = Vector2.SmoothDamp(_currentDelta, rawDelta, ref _smoothDeltaVelocity, smoothTime);
                RotateCamera(_currentDelta);
            }
        }
    }

    private void RotateCamera(Vector2 delta)
    {
        float horizontalRotation = delta.x * sensitivity;
        playerBody.Rotate(Vector3.up, horizontalRotation);

        float verticalRotation = -delta.y * sensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation + verticalRotation, -verticalClamp, verticalClamp);
        playerHead.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
    }

    private bool IsPointerInRightHalf(Vector2 pointerPosition)
    {
        return pointerPosition.x > Screen.width / 2; 
    }

    private bool IsPointerOverUI(Vector2 pointerPosition)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = pointerPosition
        };

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        return raycastResults.Count > 0; 
    }
}