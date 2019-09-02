using UnityEngine;
using DG.Tweening;
using TMPro;

public class InputPlaceholderController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _inputField;
    [SerializeField]
    private Transform _placeholderTransform;
    [SerializeField]
    private float _minPlaceholderScale = 0.4f;
    [SerializeField]
    private float _scaleSpeed = 5.0f;

    private float _maxPlaceholderScale;
    private Tween _tween;

    private void Awake()
    {
        _maxPlaceholderScale = _placeholderTransform.localScale.x;
    }

    private void ScalePlaceholder(float endScale)
    {
        var curScale = _placeholderTransform.localScale.x;
        var duration = Mathf.Abs(endScale - curScale) / _scaleSpeed;
        var scale = new Vector3(endScale, endScale, 1.0f);

        _tween.KillSafe();
        _tween = _placeholderTransform.DOScale(scale, duration);
    }

    private void OnFocusChanged(bool state)
    {
        if (string.IsNullOrEmpty(_inputField.text))
        {
            var endScale = state
                         ? _minPlaceholderScale
                         : _maxPlaceholderScale;
            ScalePlaceholder(endScale);
        }
    }

    #region Callbacks
    public void OnFocus(string _)
    {
        OnFocusChanged(true);
    }

    public void OnFocusOut(string _)
    {
        OnFocusChanged(false);
    }
    #endregion
}
