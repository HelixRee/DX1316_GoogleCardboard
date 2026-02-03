using UnityEngine;
using UnityEngine.UI;

public class QualitySettingsManager : MonoBehaviour
{
    [SerializeField] private int startQuality = 0;
    private void Start()
    {
        ChangeQualityLevel(startQuality);   
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    public void ChangeQualityLevel(int levelIndex)
    {
        // Change the quality level, optionally applying expensive changes immediately
        QualitySettings.SetQualityLevel(levelIndex, true);
    }
}