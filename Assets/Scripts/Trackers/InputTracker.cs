using System;
using UnityEngine;
using TMPro;

public class InputTracker : MonoBehaviour
{
    public event Action<bool, string> onFocusChanged;
    public event Action<Vector2, string> onCaretMoved;

    [SerializeField]
    private TMP_InputField _inputField;

    private int _prevCaretPosition = int.MinValue;
    private TMP_Text _textField;
    private RectTransform _textRectTransform;

    public TMP_InputField inputField => _inputField;

    private void Awake()
    {
        _textField = _inputField.textComponent;
        _textRectTransform = _textField.transform as RectTransform;
        enabled = _inputField.isFocused;
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Update()
    {
        var caretPosition = _inputField.caretPosition;
        if (_prevCaretPosition != caretPosition)
        {
            _prevCaretPosition = caretPosition;
            var textInfo = _textField.textInfo;
            if (caretPosition < textInfo.characterCount)
            {
                var characterInfo = textInfo.characterInfo[caretPosition];
                var originPosition = _textRectTransform.TransformPoint(new Vector3(characterInfo.origin, characterInfo.baseLine));
                onCaretMoved?.Invoke(originPosition, _textField.text);
            }
        }
    }

    private void OnSelect(string text)
    {
        enabled = true;
        onFocusChanged?.Invoke(true, text);
    }

    private void OnDeselect(string text)
    {
        onFocusChanged?.Invoke(false, text);
        enabled = false;
    }

    private void Subscribe()
    {
        _inputField.onSelect.AddListener(OnSelect);
        _inputField.onDeselect.AddListener(OnDeselect);
    }

    private void Unsubscribe()
    {
        _inputField.onSelect.RemoveListener(OnSelect);
        _inputField.onDeselect.RemoveListener(OnDeselect);
    }
}
