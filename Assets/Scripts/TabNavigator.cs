using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabNavigator : MonoBehaviour
{
    private struct Neighbours
    {
        public Selectable prev;
        public Selectable next;

        public Neighbours(Selectable prev, Selectable next)
        {
            this.prev = prev;
            this.next = next;
        }
    }

    [SerializeField]
    private Selectable[] _selectables;
    [SerializeField]
    private FocusTracker _focusTracker;

    private Dictionary<Selectable, Neighbours> _navigation = new Dictionary<Selectable, Neighbours>();
    private Selectable _focused;
    private Selectable _first;
    private Selectable _last;

    private void Start()
    {
        _focusTracker.onFocusChanged += OnFocusChanged;
        InitNavigation();
    }

    private void OnDestroy()
    {
        _focusTracker.onFocusChanged -= OnFocusChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_focused == null)
            {
                if (_last == null)
                {
                    if (_first == null)
                        return;
                    else
                        _first.Select();
                }
                else
                    _last.Select();
            }
            else
            {
                if (_navigation.TryGetValue(_focused, out var neighbours))
                {
                    Selectable target;
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        target = neighbours.prev;
                    else
                        target = neighbours.next;
                    target.Select();
                }
            }
        }
    }

    private void OnFocusChanged(Selectable selectable)
    {
        _last = _focused;
        _focused = selectable;
    }

    private void InitNavigation()
    {
        var count = _selectables.Length;
        for (int i = 0; i < count; ++i)
        {
            var currSelectable = _selectables[i];
            var nextSelectable = _selectables[(i + 1) % count];
            var prevSelectable = _selectables[((i - 1) % count + count) % count];
            _navigation.Add(currSelectable, new Neighbours(prevSelectable, nextSelectable));
        }
        if (count > 0)
            _first = _selectables[0];
    }
}
