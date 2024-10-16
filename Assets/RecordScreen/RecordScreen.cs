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
        streamButton = root.Q<Button>("Stream");
        recordButton = root.Q<Button>("Record");
        
        streamButton.clicked += OnStreamButtonPressed;
        recordButton.clicked += OnRecordButtonPressed;
    }

    private void Start()
    {
        view.style.backgroundImage = cameraManager.videoData.Texture;
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
            view.style.backgroundImage = cameraManager.videoData.Texture;
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
