using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private InputHandler _input;
    [SerializeField] private Camera _POICamera;
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

    private Queue<GameObject> _pictureObjects = new();


    private void Awake()
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
        POIManager.Instance.Photoshoot();
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

        _pictureObjects.Enqueue(pictureObj);
        if (_pictureObjects.Count > 5)
        {
            Destroy(_pictureObjects.Dequeue());
        }

        Destroy(renderTexture);
    }

    public void TakeScreenshotOf(ref Picture picture, Transform target)
    {
        _camera.cullingMask = _captureLayerMask;
        Quaternion cameraRot = _camera.transform.rotation;
        _camera.transform.LookAt(target);

        _camera.targetTexture = new RenderTexture(512, 512, 16);
        RenderTexture renderTexture = _camera.targetTexture;
        _camera.Render();
        _camera.cullingMask = _defaultLayerMask;
        _camera.targetTexture = null;

        picture.SetImageTex(renderTexture);
        _camera.transform.rotation = cameraRot;
        Destroy(renderTexture);
    }
}
