using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class POIManager : MonoBehaviour
{
    public static POIManager Instance { get; private set; } // Global access point
    [SerializeField] private CameraManager _cameraManager;
    public float leniency = 0.3f;
    [SerializeField] private List<Transform> POIs = new();

    [HideInInspector] public UnityEvent OnPhotoShot;

    [SerializeField] private TMP_Text _scoreText;
    private int _currentScore = 0;
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

    private void Update()
    {
        _scoreText.text = string.Format("Score: {0}", _currentScore);
    }

    private Transform GetRandomPOI(Transform storedPOI)
    {
        Transform randomPOI = storedPOI;
        while (randomPOI == storedPOI)
        {
            randomPOI = POIs[Random.Range(0, POIs.Count)];
        }
        return randomPOI;
    }

    public Transform SetPOIImage(ref Picture picture, Transform storedPOI)
    {
        Transform POI = GetRandomPOI(storedPOI);
        _cameraManager.TakeScreenshotOf(ref picture, POI);
        return POI;
    }

    public void Photoshoot()
    {
        OnPhotoShot?.Invoke();
    }

    public void AddPoint()
    {
        _currentScore++;
    }
}
