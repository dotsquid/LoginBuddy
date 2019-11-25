using System.Collections;
using UnityEngine;

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
    [Header("Form")]
    [SerializeField]
    private EmailTracker _emailTracker;
    [SerializeField]
    private PasswordTracker _passwordTracker;

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
        yield return new WaitForSeconds(_restartDelay);

        ResetForm();
        _animator.SetBool(kIsShownBoolHash, true);
        _restartCoroutine = null;
    }

    private void ResetForm()
    {
        _emailTracker.EmptyContent();
        _passwordTracker.EmptyContent();
    }
}
