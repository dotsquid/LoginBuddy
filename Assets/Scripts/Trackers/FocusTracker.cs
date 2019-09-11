using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusTracker : MonoBehaviour
{
    public event Action<Selectable> onFocusChanged;

    private EventSystem _eventSystem;
    private GameObject _currentSelected;

    private void Start()
    {
        _eventSystem = EventSystem.current;
    }

    private void Update()
    {
        var currentSelected = _eventSystem.currentSelectedGameObject;
        if (_currentSelected != currentSelected)
        {
            _currentSelected = currentSelected;
            Selectable selectable = null;
            if (_currentSelected != null)
            {
                selectable = _currentSelected.GetComponent<Selectable>();
            }
            onFocusChanged?.Invoke(selectable);
        }
    }
}
