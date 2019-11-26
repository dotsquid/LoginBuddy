using System.Collections;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    private const string kHiddenStateName = "Hidden";
    private static readonly int kIsShownBoolHash = Animator.StringToHash("IsShown");

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private float _showDelay = 1.0f;
    [SerializeField]
    private float _restartDelay = 0.5f;
    [Header("Restart")]
    [SerializeField]
    private BuddyController _buddyController;
    [SerializeField]
    private TMP_InputField _emailInputField;
    [SerializeField]
    private TMP_InputField _passwordInputField;

    private Coroutine _restartCoroutine;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_showDelay);
        _animator.SetBool(kIsShownBoolHash, true);
    }

    public void Restart()
    {
        if (_restartCoroutine == null)
            _restartCoroutine = StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        _animator.SetBool(kIsShownBoolHash, false);

        bool isHidden = false;
        while (!isHidden)
        {
            isHidden = _animator.GetCurrentAnimatorStateInfo(0).IsName(kHiddenStateName);
            yield return null;
        }
        RestartInternal();

        yield return new WaitForSeconds(_restartDelay);
        _animator.SetBool(kIsShownBoolHash, true);
        _restartCoroutine = null;
    }

    private void RestartInternal()
    {
        var empty = string.Empty;
        _emailInputField.text = empty;
        _passwordInputField.text = empty;
        _buddyController.Restart();
    }
}
