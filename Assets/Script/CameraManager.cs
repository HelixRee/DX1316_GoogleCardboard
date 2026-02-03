using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private InputHandler _input;
    private Camera _camera;

    [Header("Zoom")]
    [SerializeField] private float _defaultFOV = 60f;
    [SerializeField] private float _zoomedFOV = 30f;
    [SerializeField] private float _zoomSmoothSpeed = 12f;

    [Header("Screenshot")]
    [SerializeField] private GameObject _picturePrefab;
    [SerializeField] private Transform _spawnLocation;
    [SerializeField] private LayerMask _captureLayerMask;
    [SerializeField] private LayerMask _defaultLayerMask;
    [SerializeField] private float _pictureLaunchPower = 10f;
    [SerializeField] private float _pictureSpinPower = 10f;
    private bool takeScreenshotOnNextFrame = false;


    private void Start()
    {
        _camera = GetComponent<Camera>();
        _input.onHoldTrigger.AddListener(TakeScreenshot);
    }
    private void Update()
    {
        if (_input.zoomEnabled)
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomedFOV, Time.deltaTime * _zoomSmoothSpeed);
        else
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _defaultFOV, Time.deltaTime * _zoomSmoothSpeed);
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
