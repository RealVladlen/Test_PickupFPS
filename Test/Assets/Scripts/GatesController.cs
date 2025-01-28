using DG.Tweening;
using UnityEngine;

public class GatesController : MonoBehaviour
{
    [SerializeField] private Transform gates;
    [SerializeField] private float duration = 3;

    private Tween _scaleAnimation;
    private Tween _moveAnimation;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
            Open();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
            Close();
    }
    
    private void Open() => Animation(0.5f, 4.75f);

    private void Close() => Animation(5f, 2.5f);

    private void Animation(float scale, float move)
    {
        _scaleAnimation?.Kill();
        _moveAnimation?.Kill();
        
        _scaleAnimation = gates.DOScaleX(scale, duration);
        _moveAnimation = gates.DOLocalMoveY(move, duration);
    }
    
    private void OnDestroy()
    {
        _scaleAnimation?.Kill();
        _moveAnimation?.Kill();
    }
}
