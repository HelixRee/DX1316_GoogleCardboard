using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class POIManager : MonoBehaviour
{
    public static POIManager Instance { get; private set; } // Global access point
    [SerializeField] private CameraManager _cameraManager;
    public float leniency = 0.3f;
    [SerializeField] private List<Transform> POIs = new();

    [HideInInspector] public UnityEvent OnPhotoShot;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
            //DontDestroyOnLoad(gameObject); // Optional: keeps the object across scene loads
        }
        else
        {
            Destroy(gameObject); // Destroys duplicate instances
        }
    }

    private void Init()
    {
        POIs.Clear();
        foreach (Transform t in transform)
        {
            POIs.Add(t);
        }
    }

    private Transform GetRandomPOI()
    {
        return POIs[Random.Range(0, POIs.Count)];
    }

    public Transform SetPOIImage(ref Picture picture)
    {
        Transform POI = GetRandomPOI();
        _cameraManager.TakeScreenshotOf(ref picture, POI);
        return POI;
    }

    public void Photoshoot()
    {
        OnPhotoShot?.Invoke();
    }
}
