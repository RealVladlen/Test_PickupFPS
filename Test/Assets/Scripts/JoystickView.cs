using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class JoystickView : MonoBehaviour
{
    [SerializeField] private Transform stickTransform;
    [SerializeField] private Canvas canvas;

    private InputsManager _inputsManager;
    private RectTransform _joystickRect;
    private Vector2 _startClickPosition;
    private Vector2 _value;
    private float _radius;
    private CanvasGroup _canvasGroup;
    private bool _isInteractingWithButton;
    private bool _isJoystickActive;

    private void Start()
    {
        _inputsManager = InputsManager.Instance;
        _joystickRect = GetComponent<RectTransform>();
        JoystickStartPosition();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _radius = _joystickRect.sizeDelta.x * canvas.scaleFactor / 2f;
    }

    private void Update()
    {
        if (IsPointerDown() && IsPointerInLeftHalf())
        {
            _isInteractingWithButton = IsPointerOverUI() || IsPointerOverWindowView();
            if (_isInteractingWithButton)
                return;

            _isJoystickActive = true;
            _startClickPosition = GetPointerPosition();
            transform.position = _startClickPosition;
            _inputsManager.SetInputType(EInputType.Joystick);
            _canvasGroup.alpha = 1;
        }

        if (_isJoystickActive && IsPointerHeld())
        {
            if (_isInteractingWithButton)
                return;

            Vector2 delta = GetPointerPosition() - _startClickPosition;
            Vector2 clamped = Vector2.ClampMagnitude(delta, _radius);
            stickTransform.position = _startClickPosition + clamped;
            _value = clamped / _radius;
            Vector3 newValue = new Vector3(_value.x, 0, _value.y);

            _inputsManager.SetMoveDirection(newValue);
        }

        if (IsPointerUp())
        {
            _isInteractingWithButton = false;
            _isJoystickActive = false;
            JoystickStartPosition();
            _inputsManager.SetInputType(EInputType.PC);
            _canvasGroup.alpha = 0;
        }
    }

    private void JoystickStartPosition()
    {
        transform.position = new Vector3(Screen.width / 2, Screen.height / 5);
        stickTransform.localPosition = Vector3.zero;
        _value = Vector2.zero;
        _inputsManager.SetMoveDirection(_value);
    }

    private bool IsPointerOverUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = GetPointerPosition()
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPointerOverWindowView()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = GetPointerPosition()
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<FrameNotTouch>() != null)
                return true;
        }

        return false;
    }

    private Vector2 GetPointerPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;

        return Input.mousePosition;
    }

    private bool IsPointerInLeftHalf()
    {
        Vector2 pointerPosition = GetPointerPosition();
        return pointerPosition.x <= Screen.width / 2;
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
