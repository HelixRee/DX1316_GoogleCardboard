using UnityEngine;

public class POICapturer : MonoBehaviour
{
    [SerializeField] private Picture _picture;
    public Transform storedPOI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetPhoto();
        POIManager.Instance.OnPhotoShot.AddListener(CheckPhoto);
    }

    private void ResetPhoto()
    {
        storedPOI = POIManager.Instance.SetPOIImage(ref _picture);
    }

    private void CheckPhoto()
    {
        Vector3 CamToPOI = (storedPOI.position - Camera.main.transform.position).normalized;
        float score = Vector3.Dot(CamToPOI, Camera.main.transform.forward);
        if (score >= 1 - POIManager.Instance.leniency)
        {
            //success
            ResetPhoto();
            Debug.Log("Success " + score);
        }
    }
}
