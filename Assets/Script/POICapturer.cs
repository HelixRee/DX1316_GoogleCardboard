using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class POICapturer : MonoBehaviour
{
    private bool isActive = true;
    [SerializeField] private Picture _picture;
    [SerializeField] private Image _progressGauge;
    public Transform storedPOI;
    public float timeElapsed = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetPhoto();
        POIManager.Instance.OnPhotoShot.AddListener(CheckPhoto);
    }

    private void ResetPhoto()
    {
        storedPOI = POIManager.Instance.SetPOIImage(ref _picture, storedPOI);
        timeElapsed = 0f;
        isActive = true;
    }

    private void CheckPhoto()
    {
        Vector3 CamToPOI = (storedPOI.position - Camera.main.transform.position).normalized;
        float score = Vector3.Dot(CamToPOI, Camera.main.transform.forward);
        if (score >= 1 - POIManager.Instance.leniency)
        {
            //success
            POIManager.Instance.AddPoint();
            StartCoroutine(WaitAndReset());
        }
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        float fillAmt = 1 - (timeElapsed / 5);
        _progressGauge.fillAmount = fillAmt;

        if (fillAmt <= 0)
        {
            if (isActive)
                StartCoroutine(WaitAndReset());
            isActive = false;
        }

        if (!isActive)
        {
            float refillAmt = ((timeElapsed - 5) / _resetCooldown);
            _progressGauge.fillAmount = refillAmt;
        }
    }

    private float _resetCooldown = 1f;
    private IEnumerator WaitAndReset()
    {
        yield return new WaitForSeconds(_resetCooldown);
        ResetPhoto();
    }
}
