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
    private Transform _uiTransform;
    [SerializeField]
    private Transform _eyesNormalCenter;
    [SerializeField]
    private float _lookForce = 1.2f;
    [SerializeField]
    private float _maxLookDistance = 700.0f;

    private void Awake()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private (float, float) GetLookParams(Vector2 targetPosition)
    {
        var dir = targetPosition - (Vector2)_eyesNormalCenter.position;
        var angle = Mathf.Atan2(dir.y, dir.x);
        var distance = dir.magnitude / _uiTransform.lossyScale.x;
        return (angle, distance);
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
        var (angle, distance) = GetLookParams(position);
        var distanceFactor = distance / _maxLookDistance * _lookForce;
        var horizontal = Mathf.Clamp(Mathf.Cos(angle) * distanceFactor, -1.0f, 1.0f);
        var vertical = Mathf.Clamp(Mathf.Sin(angle) * distanceFactor, -1.0f, 1.0f);
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
