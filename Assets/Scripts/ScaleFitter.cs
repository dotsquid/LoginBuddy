using UnityEngine;

[ExecuteAlways]
public class ScaleFitter : MonoBehaviour
{
    [SerializeField]
    private RectTransform _source;
    [SerializeField]
    private bool _preserveAspect;

    private RectTransform _target;

    private void Awake()
    {
        _target = transform as RectTransform;
    }

    private void OnValidate()
    {
        enabled = _source != null;
    }

    private void Update()
    {
        var targetSize = _target.rect.size;
        if (targetSize.x > 0.0f && targetSize.y > 0.0f)
        {
            var sourceSize = _source.rect.size;
            var scale = sourceSize / targetSize;
            if (_preserveAspect)
                scale.x = scale.y = Mathf.Min(scale.x, scale.y);
            _target.localScale = scale;
        }
    }
}
