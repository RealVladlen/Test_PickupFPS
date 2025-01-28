using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [SerializeField] private Transform rayTransform;
    [SerializeField] private float interactionDistance = 3f; 
    [SerializeField] private float throwForce = 5f; 
    [SerializeField] private float throwForceUp = 0.5f; 
    [SerializeField] private Transform handPosition; 
    [SerializeField] private LayerMask itemLayer; 
    [SerializeField] private float smoothTime = 0.2f; 

    private GameObject _currentItem; 
    private Rigidbody _itemRigidbody; 
    private bool _isItemHeld = false; 
    private Vector3 _velocity = Vector3.zero; 

    private void Update()
    {
        if (!_isItemHeld)
            CheckForItem();

        if (_isItemHeld && _currentItem != null)
        {
            Vector3 targetPosition = handPosition.position;
            _itemRigidbody.MovePosition(Vector3.SmoothDamp(_currentItem.transform.position, targetPosition, ref _velocity, smoothTime));
        }
    }

    private void CheckForItem()
    {
        RaycastHit hit;
        
        Vector3 rayOrigin = rayTransform.position;
        Vector3 rayDirection = rayTransform.forward * interactionDistance;

        Debug.DrawRay(rayOrigin, rayDirection, Color.red);
        
        if (Physics.Raycast(rayTransform.position, rayTransform.forward, out hit, interactionDistance, itemLayer))
        {
            if (hit.collider.TryGetComponent(out Item item))
            {
                if (_currentItem != null) return; 
                
                _currentItem = hit.collider.gameObject;
                _itemRigidbody = _currentItem.GetComponent<Rigidbody>();

                UIController.Instance.ShowButton();
                UIController.Instance.SetAction(PickUpItem);
                UIController.Instance.SetTextButton("Take");
            }
        }
        else if (!_isItemHeld && _currentItem != null)
        {
            UIController.Instance.HideButton();
            UIController.Instance.ClearAction();
            ClearCurrentItem();
        }
    }

    private void ClearCurrentItem()
    {
        _currentItem = null;
        _itemRigidbody = null;
    }

    private void PickUpItem()
    {
        if (_currentItem != null)
        {
            _isItemHeld = true;

            _itemRigidbody.useGravity = false; 
            _itemRigidbody.constraints = RigidbodyConstraints.FreezeRotation; 

            UIController.Instance.SetAction(ThrowItem);
            UIController.Instance.SetTextButton("Throw");
        }
    }

    private void ThrowItem()
    {
        if (_currentItem != null)
        {
            _isItemHeld = false;

            _itemRigidbody.useGravity = true;
            _itemRigidbody.constraints = RigidbodyConstraints.None;

            _itemRigidbody.AddForce((transform.forward + Vector3.up * throwForceUp) * throwForce , ForceMode.Impulse);

            UIController.Instance.HideButton();
            UIController.Instance.ClearAction();
            ClearCurrentItem();
        }
    }
}