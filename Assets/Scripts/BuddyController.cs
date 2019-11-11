using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BuddyController : MonoBehaviour
{
    private static readonly int kHorizontalFloatHash = Animator.StringToHash("Horizontal");
    private static readonly int kVerticalFloatHash = Animator.StringToHash("Vertical");
    private static readonly int kIsHidingBoolHash = Animator.StringToHash("IsHiding");
    private static readonly int kIsPryingBoolHash = Animator.StringToHash("IsPrying");
    private static readonly int kExcitementIntHash = Animator.StringToHash("Excitement");
    private static readonly int kBlinkEyesTriggerHash = Animator.StringToHash("BlinkEyes");

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private EmailTracker _emailTracker;
    [SerializeField]
    private PasswordTracker _passwordTracker;
    [SerializeField]
    private PasswordVisibilityController _passwordVisibility;
    [SerializeField]
    private FocusTracker _focusTracker;
    [SerializeField]
    private Selectable[] _passwordSelectables;
    [SerializeField]
    private Transform _uiTransform;
    [SerializeField]
    private Transform _eyesNormalCenter;
    [SerializeField]
    private Vector2 _lookVelocity = new Vector2(1.0f, 1.0f);
    [SerializeField]
    private float _lookForce = 1.2f;
    [SerializeField]
    private float _maxLookDistance = 700.0f;
    [Header("Blinkig")]
    [SerializeField, Range(0.0f, 1.0f)]
    private float _doubleBlinkProbability = 0.1f;
    [SerializeField]
    private float _minBlinkPeriod = 1.0f;
    [SerializeField]
    private float _maxBlinkPeriod = 5.0f;
    [SerializeField]
    private AnimationCurve _blinkPeriodDistibution = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    private HashSet<Selectable> _passwordSelectablesSet;
    private Tween _horizontalTween;
    private Tween _verticalTween;

    private void Awake()
    {
        InitPasswordSelectable();
        StartCoroutine(BlinkCoroutine());
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void InitPasswordSelectable()
    {
        _passwordSelectablesSet = new HashSet<Selectable>(_passwordSelectables);
    }

    private (float angle, float distance) GetLookParams(Vector2 targetPosition)
    {
        var dir = targetPosition - (Vector2)_eyesNormalCenter.position;
        var angle = Mathf.Atan2(dir.y, dir.x);
        var distance = dir.magnitude / _uiTransform.lossyScale.x;
        return (angle, distance);
    }

    private void TurnHead(float horizontal, float vertical)
    {
        var horizontalDuration = Mathf.Abs(_animator.GetFloat(kHorizontalFloatHash) - horizontal) / _lookVelocity.x;
        var verticalDuration = Mathf.Abs(_animator.GetFloat(kVerticalFloatHash) - vertical) / _lookVelocity.y;
        var duration = Mathf.Max(horizontalDuration, verticalDuration);

        _horizontalTween.KillSafe();
        _verticalTween.KillSafe();
        _horizontalTween = _animator.DOFloat(kHorizontalFloatHash, horizontal, duration);
        _verticalTween = _animator.DOFloat(kVerticalFloatHash, vertical, duration);
    }

    private void SetHandsPosition(bool isUp)
    {
        _animator.SetBool(kIsHidingBoolHash, isUp);
    }

    private void SetPryingState(bool isPrying)
    {
        _animator.SetBool(kIsPryingBoolHash, isPrying);
    }

    private void OnEmailCaretMoved(Vector2 position)
    {
        OnInputCaretMoved(position);
    }

    private void OnEmailFocusChanged(bool state, string _)
    {
        if (!state)
            OnFocusLose();
    }

    private void OnPasswordCaretMoved(Vector2 position)
    {
        //var isPasswordEmpty = text.IsPasswordEmpty();
        //SetHandsPosition(!isPasswordEmpty);
        OnInputCaretMoved(position);
    }

    private void OnPasswordFocusChanged(bool state, string text)
    {
        return;

        var isPasswordEmpty = text.IsPasswordEmpty();
        if (state)
        {
            if (!isPasswordEmpty)
                SetHandsPosition(true);
        }
        else
        {
            OnFocusLose();
            SetHandsPosition(false);
        }
    }

    private void OnInputCaretMoved(Vector2 position)
    {
        var (angle, distance) = GetLookParams(position);
        var distanceFactor = distance / _maxLookDistance * _lookForce;
        var horizontal = Mathf.Clamp(Mathf.Cos(angle) * distanceFactor, -1.0f, 1.0f);
        var vertical = Mathf.Clamp(Mathf.Sin(angle) * distanceFactor, -1.0f, 1.0f);
        TurnHead(horizontal, vertical);
    }

    private void OnEmailContentChanged(string content)
    {
        int excitement = 0;
        int atIndex = content.IndexOf('@');
        bool hasAt = atIndex > 0 && atIndex < content.Length;
        if (hasAt)
            excitement += 1;
        int dotIndex = content.IndexOf('.', hasAt ? atIndex : 0);
        if (dotIndex > atIndex + 1)
            excitement += 1;
        _animator.SetInteger(kExcitementIntHash, excitement);
    }

    private void OnFocusLose()
    {
        TurnHead(0.0f, 0.0f);
    }

    private void OnPasswordVisibilityChanged(bool isOn)
    {
        //SetHandsPosition(true);
        SetPryingState(isOn);
    }

    private void OnFocusChanged(Selectable selectable)
    {
        var isSelecteblePasswordRelated = _passwordSelectablesSet.Contains(selectable);
        SetHandsPosition(isSelecteblePasswordRelated);
    }

    private IEnumerator BlinkCoroutine()
    {
        while (true)
        {
            float periodT = _blinkPeriodDistibution.Evaluate(Random.value);
            float period = Mathf.Lerp(_minBlinkPeriod, _maxBlinkPeriod, periodT);
            yield return new WaitForSeconds(period);

            _animator.SetTrigger(kBlinkEyesTriggerHash);
            if (Random.value < _doubleBlinkProbability)
            {
                yield return null;
                _animator.SetTrigger(kBlinkEyesTriggerHash);
            }
        }
    }

    private void Subscribe()
    {
        _emailTracker.onCaretMoved += OnEmailCaretMoved;
        _emailTracker.onFocusChanged += OnEmailFocusChanged;
        _emailTracker.onContentChanged += OnEmailContentChanged;
        _passwordTracker.onCaretMoved += OnPasswordCaretMoved;
        _passwordTracker.onFocusChanged += OnPasswordFocusChanged;
        _passwordVisibility.onChange += OnPasswordVisibilityChanged;
        _focusTracker.onFocusChanged += OnFocusChanged;
    }

    private void Unsubscribe()
    {
        _emailTracker.onCaretMoved -= OnEmailCaretMoved;
        _emailTracker.onFocusChanged -= OnEmailFocusChanged;
        _emailTracker.onContentChanged -= OnEmailContentChanged;
        _passwordTracker.onCaretMoved -= OnPasswordCaretMoved;
        _passwordTracker.onFocusChanged -= OnPasswordFocusChanged;
        _passwordVisibility.onChange -= OnPasswordVisibilityChanged;
        _focusTracker.onFocusChanged -= OnFocusChanged;
    }
}
