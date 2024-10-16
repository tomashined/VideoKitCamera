using System;
using UnityEngine;
using UnityEngine.UIElements;
using Video;

public class RecordScreen : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private CameraManager cameraManager;

    private VisualElement root;
    private VisualElement view;
    private Button streamButton, recordButton;

    private void Awake()
    {
        root = uiDocument.rootVisualElement;
        view = root.Q<VisualElement>("View");
        recordButton = root.Q<Button>("Record");
    }
    
    public void OnStreamButtonPressed()
    {
        if (cameraManager.IsStreaming)
        {
            cameraManager.StopStreaming();
            streamButton.text = "Start Streaming";
        }
        else
        {
            cameraManager.StartStreaming();
            streamButton.text = "Stop Streaming";
        }
    }

    public void OnRecordButtonPressed()
    {
        if (cameraManager.IsRecording)
        {
            cameraManager.StopRecording();
            recordButton.text = "Start Recording";
        }
        else
        {
            cameraManager.StartRecording();
            recordButton.text = "Stop Recording";
        }
    }
}
