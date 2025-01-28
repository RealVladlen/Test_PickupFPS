using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController _characterController;
    private Vector3 _moveDirection;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform cameraTransform;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        _moveDirection = InputsManager.Instance.GetMoveDirection;

        if (_moveDirection != Vector3.zero)
            MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 direction = new Vector3(_moveDirection.x, 0f, _moveDirection.z);

        direction = cameraTransform.TransformDirection(direction);

        direction.y = 0f;
        direction.Normalize();

        _characterController.Move(direction * moveSpeed * Time.deltaTime);
    }
}