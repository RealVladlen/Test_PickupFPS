using UnityEngine;

public enum EInputType
{
    Joystick,
    PC
}
public class InputsManager : MonoBehaviour
{
    public static InputsManager Instance;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
    }

    private Vector3 _moveValue;
    private EInputType _type = EInputType.PC;

    public EInputType GetInputType => _type;
    public void SetInputType(EInputType type) => _type = type;
    public Vector3 GetMoveDirection => _moveValue;
    
    public void SetMoveDirection(Vector3 direction) => _moveValue = direction;

    private void OnDestroy()
    {
        Instance = null;
    }
}