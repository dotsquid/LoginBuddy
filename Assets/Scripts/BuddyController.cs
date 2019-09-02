using UnityEngine;

public class BuddyController : MonoBehaviour
{
    private static readonly int kHorizontalFloatHash = Animator.StringToHash("Horizontal");
    private static readonly int kVerticalFloatHash = Animator.StringToHash("Vertical");

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private EmailTracker _emailTracker;
    [SerializeField]
    private PasswordTracker _passwordTracker;
    [SerializeField]
    private Transform _eyesNormalCenter;
    [SerializeField]
    private float _lookForce = 1.2f;

    private void Awake()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private float GetLookAngle(Vector2 targetPosition)
    {
        var dir = targetPosition - (Vector2)_eyesNormalCenter.position;
        return Mathf.Atan2(dir.y, dir.x);
    }

    private void OnEmailCaretMoved(Vector2 position)
    {
        OnInputCaretMoved(position);
    }

    private void OnEmailFocusChanged(bool state)
    { }

    private void OnPasswordCaretMoved(Vector2 position)
    {
        OnInputCaretMoved(position);
    }

    private void OnPasswordFocusChanged(bool state)
    { }

    private void OnInputCaretMoved(Vector2 position)
    {
        var angle = GetLookAngle(position);
        var horizontal = Mathf.Clamp(Mathf.Cos(angle) * _lookForce, -1.0f, 1.0f);
        var vertical = Mathf.Clamp(Mathf.Sin(angle) * _lookForce, -1.0f, 1.0f);
        _animator.SetFloat(kHorizontalFloatHash, horizontal);
        _animator.SetFloat(kVerticalFloatHash, vertical);
    }

    private void Subscribe()
    {
        _emailTracker.onCaretMoved += OnEmailCaretMoved;
        _emailTracker.onFocusChanged += OnEmailFocusChanged;
        _passwordTracker.onCaretMoved += OnPasswordCaretMoved;
        _passwordTracker.onFocusChanged += OnPasswordFocusChanged;
    }

    private void Unsubscribe()
    {
        _emailTracker.onCaretMoved -= OnEmailCaretMoved;
        _emailTracker.onFocusChanged -= OnEmailFocusChanged;
        _passwordTracker.onCaretMoved -= OnPasswordCaretMoved;
        _passwordTracker.onFocusChanged -= OnPasswordFocusChanged;
    }
}
