using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordVisibilityController : MonoBehaviour
{
    public event Action<bool> onChange;

    [SerializeField]
    private Image _image;
    [SerializeField]
    private Sprite _onSprite;
    [SerializeField]
    private Sprite _offSprite;
    [SerializeField]
    private TMP_InputField _passwordField;

    private bool _isOn;

    public void OnClick()
    {
        _isOn = !_isOn;
        if (_isOn)
        {
            _image.sprite = _onSprite;
            _passwordField.inputType = TMP_InputField.InputType.Standard;
        }
        else
        {
            _image.sprite = _offSprite;
            _passwordField.inputType = TMP_InputField.InputType.Password;
        }
        _passwordField.ForceLabelUpdate();
        onChange?.Invoke(_isOn);
    }
}
