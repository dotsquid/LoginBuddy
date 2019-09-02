﻿using UnityEngine;
using DG.Tweening;
using TMPro;

public class InputPlaceholderController : MonoBehaviour
{
    [SerializeField]
    private EmailTracker _emailTracker;
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
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void ScalePlaceholder(float endScale)
    {
        var curScale = _placeholderTransform.localScale.x;
        var duration = Mathf.Abs(endScale - curScale) / _scaleSpeed;
        var scale = new Vector3(endScale, endScale, 1.0f);

        _tween.KillSafe();
        _tween = _placeholderTransform.DOScale(scale, duration);
    }

    private void OnEmailFocusChanged(bool state)
    {
        if (string.IsNullOrEmpty(_inputField.text))
        {
            var endScale = state
                         ? _minPlaceholderScale
                         : _maxPlaceholderScale;
            ScalePlaceholder(endScale);
        }
    }

    private void Subscribe()
    {
        _emailTracker.onFocusChanged += OnEmailFocusChanged;
    }

    private void Unsubscribe()
    {
        _emailTracker.onFocusChanged -= OnEmailFocusChanged;
    }
}
