using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private InputHandler _input;
    private Camera _camera;

    [Header("Zoom")]
    [SerializeField] private float _defaultOffset = 60f;
    [SerializeField] private float _zoomedOffset = 30f;
    private float _offset = 0f;
    [SerializeField] private float _zoomSmoothSpeed = 12f;

    [Header("Screenshot")]
    [SerializeField] private GameObject _picturePrefab;
    [SerializeField] private Transform _spawnLocation;
    [SerializeField] private LayerMask _captureLayerMask;
    [SerializeField] private LayerMask _defaultLayerMask;
    [SerializeField] private float _pictureLaunchPower = 10f;
    [SerializeField] private float _pictureSpinPower = 10f;


    private void Start()
    {
        _camera = GetComponent<Camera>();
        _input.onHoldTrigger.AddListener(TakeScreenshot);

    }
    private void LateUpdate()
    {
        //if (_input.zoomEnabled)
        //    _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomedOffset, Time.deltaTime * _zoomSmoothSpeed);
        //else
        //    _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _defaultOffset, Time.deltaTime * _zoomSmoothSpeed);
        if (_input.zoomEnabled)
            _offset = Mathf.Lerp(_offset, _zoomedOffset, Time.deltaTime * _zoomSmoothSpeed);
        else
            _offset = Mathf.Lerp(_offset, _defaultOffset, Time.deltaTime * _zoomSmoothSpeed);

        Vector3 pos = _camera.transform.localPosition;
        pos = _camera.transform.forward * _offset;
        _camera.transform.localPosition = pos;

    }

    private void TakeScreenshot()
    {
        _camera.cullingMask = _captureLayerMask;

        _camera.targetTexture = new RenderTexture(512, 512, 16);
        RenderTexture renderTexture = _camera.targetTexture;
        _camera.Render();
        _camera.cullingMask = _defaultLayerMask;
        _camera.targetTexture = null;

        GameObject pictureObj = Instantiate(_picturePrefab);
        pictureObj.transform.position = _spawnLocation.position;
        pictureObj.transform.rotation = _spawnLocation.rotation;
        Rigidbody pictureRB = pictureObj.GetComponent<Rigidbody>();
        pictureRB.AddForce(_camera.transform.forward * _pictureLaunchPower, ForceMode.Impulse);
        pictureRB.AddTorque(Random.onUnitSphere * _pictureSpinPower, ForceMode.Impulse);
        Picture pictureScript = pictureObj.GetComponent<Picture>();
        pictureScript.SetImageTex(renderTexture);
        Destroy(renderTexture);
    }
}
