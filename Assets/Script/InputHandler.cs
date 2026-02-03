using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InputHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _doubleTapTimeout = 0.2f;
    [SerializeField] private float _triggerHoldTimeout = 0.5f;

    [Header("Exposed Variables")]
    public bool zoomEnabled = false;
    public bool triggerOverride = false;
    public UnityEvent onHoldTrigger = new();

    private bool _triggerHeld = false;
    private bool _triggerHoldReady = true;
    private float _lastTriggerInputTimestamp = 0f;
    
    public void TriggerInput()
    {
        CheckForDoubleTap();
    }
    public void HoldTriggerInput()
    {
        if (!_triggerHoldReady) return;

        onHoldTrigger.Invoke();
        _triggerHoldReady = false;
    }
    public void DoubleTap()
    {
        zoomEnabled = !zoomEnabled;
        //Debug.Log("Double Tapped");
    }

    private void CheckForDoubleTap()
    {
        if (Time.time - _lastTriggerInputTimestamp < _doubleTapTimeout)
            DoubleTap();

        _lastTriggerInputTimestamp = Time.time;
    }

    private void Update()
    {
        if (Google.XR.Cardboard.Api.IsTriggerPressed || triggerOverride)
        {
            if (!_triggerHeld)
                TriggerInput();

            if (Time.time - _lastTriggerInputTimestamp > _triggerHoldTimeout)
                HoldTriggerInput();
            _triggerHeld = true;
        }
        else
        {
            _triggerHeld = false;
            _triggerHoldReady = true;
        }



    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InputHandler))]
public class InputHandlerInspector : Editor
{
    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();
        InputHandler inputHandler = (InputHandler)target;

        if (GUILayout.Button("Test Trigger"))
        {
            inputHandler.TriggerInput();
        }
        if (GUILayout.Button("Test Trigger Hold"))
        {
            inputHandler.HoldTriggerInput();
        }
    }
}
#endif