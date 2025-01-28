using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [SerializeField] private Button itemAction;
    [SerializeField] private TextMeshProUGUI buttonText;

    private CanvasGroup _buttonCanvasGroup;
    private Tween _fadeAnimation;
    private Action _action;
    
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;

        _buttonCanvasGroup = itemAction.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        itemAction.onClick.AddListener(Action);
        itemAction.gameObject.SetActive(false);
    }

    public void ShowButton()
    {
        itemAction.gameObject.SetActive(true);
        _buttonCanvasGroup.alpha = 0;
        _fadeAnimation?.Kill();
        _fadeAnimation = _buttonCanvasGroup.DOFade(1, 0.15f);
    }

    public void HideButton()
    {
        _fadeAnimation?.Kill();
        
        _fadeAnimation = _buttonCanvasGroup.DOFade(0, 0.15f).OnComplete(() =>
        {
            itemAction.gameObject.SetActive(false);
        });
    }

    public void SetAction(Action action) => _action = action;
    public void SetTextButton(string textInfo) => buttonText.text = textInfo;

    public void Action()
    {
        _action?.Invoke();
    }

    public void ClearAction()
    {
        _action = null;
    }

    private void OnDisable()
    {
        itemAction.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        Instance = null;
        _fadeAnimation?.Kill();
    }
}
