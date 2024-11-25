using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using Video;

public class RecordScreen : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private ReplayManager replayManager;

    private VisualElement root;
    private VisualElement view;
    private Button streamButton, recordButton, replayButton, findButton;
    private IntegerField findField;

    private Texture2D texture;
    [CreateProperty] private Texture2D Texture
    {
        get => texture;
        set
        {
            texture = value;
            view.style.backgroundImage = texture;
        }
    }

    private void Awake()
    {
        root = uiDocument.rootVisualElement;
        view = root.Q<VisualElement>("View");
        streamButton = root.Q<Button>("Stream");
        recordButton = root.Q<Button>("Record");
        replayButton = root.Q<Button>("Replay");
        findButton = root.Q<Button>("Find");
        findField = findButton.Q<IntegerField>("Find");
        root = view.parent;
        
        view.RegisterCallback<ClickEvent>(evt => SetTexture(cameraManager.CameraTexture));
        streamButton.clicked += OnStreamButtonPressed;
        recordButton.clicked += OnRecordButtonPressed;
        replayButton.clicked += () => replayManager.OnReplayButtonPressed(SetTexture);
        findButton.clicked += () => replayManager.OnFindButtonPressed(SetTexture);
        findField.RegisterValueChangedCallback(evt => replayManager.OnFindFieldChanged(evt.newValue));
    }

    private void Start()
    {
        
    }
    
    public void SetTexture(Texture2D texture) => view.style.backgroundImage = texture;

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

    public void ShowTexture()
    {
        view.style.backgroundImage = cameraManager.CameraTexture;
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
