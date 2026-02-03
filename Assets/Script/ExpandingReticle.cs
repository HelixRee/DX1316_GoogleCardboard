using UnityEngine;

public class ExpandingReticle : MonoBehaviour
{
    [SerializeField] private InputHandler _input;
    [SerializeField] private float _minSize = 100f;
    [SerializeField] private float _maxSize = 500f;
    [SerializeField] private float _smoothingSpeed = 12f; 
    [SerializeField] private RectTransform _rectTransform;

    // Update is called once per frame
    void Update()
    {
        if (_input.zoomEnabled)
        {
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(_rectTransform.rect.width, _maxSize, Time.deltaTime * _smoothingSpeed));
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(_rectTransform.rect.height, _maxSize, Time.deltaTime * _smoothingSpeed));
        }
        else
        {
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(_rectTransform.rect.width, _minSize, Time.deltaTime * _smoothingSpeed));
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(_rectTransform.rect.height, _minSize, Time.deltaTime * _smoothingSpeed));
        }
    }
}
