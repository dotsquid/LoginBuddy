using UnityEngine;
using UnityEngine.UI;

public class PasswordVisibilityController : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Sprite _onSprite;
    [SerializeField]
    private Sprite _offSprite;

    private bool _isOn;

    public void OnClick()
    {
        _isOn = !_isOn;
        _image.sprite = _isOn
                      ? _onSprite
                      : _offSprite;
    }
}
