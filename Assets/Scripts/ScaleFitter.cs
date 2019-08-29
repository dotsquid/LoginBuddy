using UnityEngine;

[ExecuteAlways]
public class ScaleFitter : MonoBehaviour
{
    [SerializeField]
    private RectTransform _source;

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
        var sourceSize = _source.rect.size;
        var targetSize = _target.rect.size;
        var scale = sourceSize / targetSize;
        _target.localScale = scale;
    }
}
