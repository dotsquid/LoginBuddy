using System;
using UnityEngine;
using TMPro;

public class InputTracker : MonoBehaviour
{
    public event Action<string> onContentChanged;
    public event Action<bool, string> onFocusChanged;
    public event Action<Vector2> onCaretMoved;

    [SerializeField]
    private TMP_InputField _inputField;

    private string _prevContent = string.Empty;
    private int _prevContentHash = string.Empty.GetHashCode();
    private int _prevContentLength = 0;
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
        CheckCaret();
        CheckContent();
    }

    public void EmptyContent()
    {
        _inputField.text = string.Empty;
        if (!isActiveAndEnabled)
            Update();
    }

    private void CheckCaret()
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
                onCaretMoved?.Invoke(originPosition);
            }
        }
    }

    private void CheckContent()
    {
        var content = _textField.text;
        var contentLength = content.Length;
        if (contentLength != _prevContentLength)
        {
            var contentHash = content.GetHashCode();
            bool isSame = (contentHash == _prevContentHash)
                       && (content == _prevContent);
            if (!isSame)
            {
                _prevContent = content;
                _prevContentHash = contentHash;
                _prevContentLength = contentLength;
                onContentChanged?.Invoke(content);
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
