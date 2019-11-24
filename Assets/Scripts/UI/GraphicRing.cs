using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace SC.Utilities
{
    public class GraphicRing : Graphic
    {
        public enum SmoothnessMode
        {
            None,
            Inside,
            Outside,
            Both
        }

        private const int kMinSegmentNum = 3;
        private const float kTangentialRadiusFactor = 1.4142135623f;

        [SerializeField]
        private int _segmentNum = 64;

        [SerializeField]
        private float _thickness = 1.0f;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _fillAmount = 1.0f;

        [Tooltip("0.0 corresponds to 0 degrees (top). 0.25 - 90 degrees (left or right)")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _fillStart = 1.0f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _smoothness = 0.5f;

        [SerializeField]
        private SmoothnessMode _smoothnessMode = SmoothnessMode.Both;

        [SerializeField]
        private bool _isClockwise = true;

        [SerializeField]
        private bool _isTangential = false;

        public override Texture mainTexture
        {
            get
            {
                if (material != null && material.mainTexture != null)
                {
                    return material.mainTexture;
                }
                return s_WhiteTexture;
            }
        }

        public int segmentNum
        {
            get { return _segmentNum; }
            set
            {
                _segmentNum = Mathf.Clamp(value, kMinSegmentNum, int.MaxValue);
                SetVerticesDirty();
            }
        }

        public float thickness
        {
            get { return _thickness; }
            set
            {
                _thickness = value;
                SetVerticesDirty();
            }
        }

        public float fillAmount
        {
            get { return _fillAmount; }
            set
            {
                _fillAmount = Mathf.Clamp01(value);
                SetVerticesDirty();
            }
        }

        public float fillStart
        {
            get { return _fillStart; }
            set
            {
                _fillStart = Mathf.Clamp01(value);
                SetVerticesDirty();
            }
        }

        public float smoothness
        {
            get { return _smoothness; }
            set
            {
                _smoothness = value;
                SetVerticesDirty();
            }
        }

        public SmoothnessMode smoothnessMode
        {
            get { return _smoothnessMode; }
            set
            {
                _smoothnessMode = value;
                SetVerticesDirty();
            }
        }

        public bool isClockwise
        {
            get { return _isClockwise; }
            set
            {
                _isClockwise = value;
                SetVerticesDirty();
            }
        }

        public bool isTangential
        {
            get { return _isTangential; }
            set
            {
                _isTangential = value;
                SetVerticesDirty();
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _segmentNum = Mathf.Max(_segmentNum, kMinSegmentNum);
            _smoothness = Mathf.Clamp01(_smoothness);
        }
#endif
        // FIXME: when the fillAmount < 1.0f
        // it's noticeable that the edge of the sector
        // is not smoothed
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Color transparent = color;
            transparent.a = 0.0f;

            bool isInsideSmooth = false;
            bool isOutsideSmooth = false;
            switch (_smoothnessMode)
            {
                case SmoothnessMode.Inside:
                    if (_smoothness > 0.0)
                        isInsideSmooth = true;
                    break;

                case SmoothnessMode.Outside:
                    if (_smoothness > 0.0)
                        isOutsideSmooth = true;
                    break;

                case SmoothnessMode.Both:
                    if (_smoothness > 0.0)
                    {
                        isInsideSmooth = true;
                        isOutsideSmooth = true;
                    }
                    break;
            }

            vh.Clear();
            UIVertex vert = UIVertex.simpleVert;

            Rect rect = rectTransform.rect;
            Vector2 center = rect.center;
            Vector2 halfRectSize = rect.size * 0.5f;

            // 0.25f is required to make fillStart == 0.0f correspond 12 o'clock
            float startAlpha = 2.0f * Mathf.PI * (_fillStart + 0.25f);
            float theta = 2.0f * Mathf.PI / _segmentNum;
            float radiusFactor = _isTangential ? kTangentialRadiusFactor : 1.0f;
            float sign = _isClockwise ? +1.0f : -1.0f;
            int segmentNum = Mathf.CeilToInt(_segmentNum * _fillAmount);
            Vector2 prevPos = default;

            for (int i = 0; i <= segmentNum; ++i)
            {
                float u = (float)i / _segmentNum;
                float alpha = sign * theta * i + startAlpha;
                var dir = radiusFactor * new Vector2(Mathf.Cos(alpha), Mathf.Sin(alpha));
                Vector2 innerDir;
                if (i == segmentNum)
                {
                    float t = 1.0f - (_fillAmount - (i - 1.0f) / _segmentNum) * _segmentNum;
                    float innerAlpha = alpha - theta * t;
                    float middleAlpha = sign * theta * (i - 0.5f) + startAlpha;
                    float diffAlpha = Mathf.Abs(innerAlpha - middleAlpha);
                    float radiusCompensation = Mathf.Cos(theta * 0.5f) / Mathf.Cos(diffAlpha);
                    innerDir = radiusFactor * radiusCompensation * new Vector2(Mathf.Cos(innerAlpha), Mathf.Sin(innerAlpha));
                }
                else
                {
                    innerDir = dir;
                }

                var pos = Vector2.Scale(dir, halfRectSize);
                if (i == segmentNum)
                {
                    float t = (_fillAmount - (i - 1.0f) / _segmentNum) * _segmentNum;
                    pos = Vector2.Lerp(prevPos, pos, t);
                }
                prevPos = pos;
                vert.position = center + pos;
                vert.color = isOutsideSmooth ? transparent : color;
                vert.uv0 = new Vector2(u, 0.0f);
                vh.AddVert(vert);

                Vector2 innerOffset = innerDir * thickness;
                if (_smoothness > 0.0f)
                {
                    Vector2 halfInnerOffset = innerOffset * 0.5f;

                    switch (_smoothnessMode)
                    {
                        case SmoothnessMode.Outside:
                        case SmoothnessMode.Both:
                            vert.position = center + pos - halfInnerOffset * _smoothness;
                            vert.color = color;
                            vert.uv0 = new Vector2(u, _smoothness * 0.5f);
                            vh.AddVert(vert);
                            break;
                    }

                    if (_smoothness < 1.0f)
                    {
                        switch (_smoothnessMode)
                        {
                            case SmoothnessMode.Inside:
                            case SmoothnessMode.Both:
                                vert.position = center + pos - innerOffset + halfInnerOffset * _smoothness;
                                vert.color = color;
                                vert.uv0 = new Vector2(u, 1.0f - _smoothness * 0.5f);
                                vh.AddVert(vert);
                                break;
                        }
                    }
                }

                vert.position = center + pos - innerOffset;
                vert.color = isInsideSmooth ? transparent : color;
                vert.uv0 = new Vector2(u, 1.0f);
                vh.AddVert(vert);
            }

            for (int i = 0; i < segmentNum; ++i)
            {
                if (_smoothness > 0.0f && (isInsideSmooth || isOutsideSmooth))
                {
                    if (_smoothness < 1.0f && isInsideSmooth && isOutsideSmooth)
                    {
                        var i4 = i * 4;
                        vh.AddTriangle(i4 + 0, i4 + 1, i4 + 5);
                        vh.AddTriangle(i4 + 5, i4 + 4, i4 + 0);

                        vh.AddTriangle(i4 + 1, i4 + 2, i4 + 6);
                        vh.AddTriangle(i4 + 6, i4 + 5, i4 + 1);

                        vh.AddTriangle(i4 + 2, i4 + 3, i4 + 7);
                        vh.AddTriangle(i4 + 7, i4 + 6, i4 + 2);
                    }
                    else
                    {
                        var i3 = i * 3;
                        vh.AddTriangle(i3 + 0, i3 + 1, i3 + 4);
                        vh.AddTriangle(i3 + 4, i3 + 3, i3 + 0);

                        vh.AddTriangle(i3 + 1, i3 + 2, i3 + 5);
                        vh.AddTriangle(i3 + 5, i3 + 4, i3 + 1);
                    }
                }
                else
                {
                    var i2 = i * 2;
                    vh.AddTriangle(i2 + 0, i2 + 1, i2 + 3);
                    vh.AddTriangle(i2 + 3, i2 + 2, i2 + 0);
                }
            }
        }
    }
}
