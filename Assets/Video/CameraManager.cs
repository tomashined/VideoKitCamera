using System;
using UnityEngine;
using VideoKit;

namespace Video
{
    [RequireComponent(typeof(VideoKitCameraManager), typeof(VideoKitRecorder))]
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private RecordScreen recordScreen;
        
        [Header("Components")]
        [SerializeField] private VideoKitCameraManager cameraManager;
        [SerializeField] private VideoKitRecorder cameraRecorder;
        
        [Header("State")]
        [SerializeField] private bool isInitialized;
        [SerializeField] private bool isStreaming;
        [SerializeField] private bool isRecording;
        
        [Header("Camera")]
        [SerializeField] public VideoData videoData;
        [SerializeField] private CameraDevice cameraDevice;
        [SerializeField] private Texture2D cameraTexture;
        
        public bool IsStreaming => isStreaming;
        public bool IsRecording => isRecording;
        public Texture2D CameraTexture => cameraTexture;

        private void Awake()
        {
            if (!cameraManager) cameraManager = GetComponent<VideoKitCameraManager>();
            if (!cameraRecorder) cameraRecorder = GetComponent<VideoKitRecorder>();
            
            cameraManager.playOnAwake = false;
            cameraManager.OnCameraFrame?.AddListener(OnCameraFrame);

            cameraRecorder.format = MediaRecorder.Format.HEVC;
            cameraRecorder.prepareOnAwake = false;
            cameraRecorder.videoMode = VideoKitRecorder.VideoMode.CameraDevice;
            cameraRecorder.cameraManager = cameraManager;
            cameraRecorder.recordingAction = VideoKitRecorder.RecordingAction.Custom;
            cameraRecorder.OnRecordingCompleted?.AddListener(OnRecordingCompleted);
            
            videoData = new VideoData();
        }
        private void OnCameraFrame()
        {
            if (!isInitialized)
            {
                Debug.Log($"OnCameraFrame: Initialization");
                cameraDevice = cameraManager.device;
                cameraTexture = cameraManager.texture;
                if (cameraDevice == null)
                {
                    Debug.Log($"OnCameraFrame: Camera device is null");
                    return;
                }
                if (cameraTexture == null)
                {
                    Debug.Log($"OnCameraFrame: Camera texture is null");
                    return;
                }
                
                videoData.Name = cameraDevice.name;
                videoData.Resolution = new Resolution(cameraTexture.width, cameraTexture.height, (int)cameraDevice.frameRate);
                videoData.Texture = cameraTexture;
                
                Debug.Log(JsonUtility.ToJson(videoData, true));
                
                isInitialized = true;
                isStreaming = cameraManager.running;
                
                Debug.Log($"Is streaming: {isStreaming}");
                
                recordScreen.ShowTexture();
            }
        }
        private void OnRecordingCompleted(MediaAsset mediaAsset)
        {
            Debug.Log($"OnRecordingCompleted(): {mediaAsset.width}x{mediaAsset.height}@{mediaAsset.frameRate} - {mediaAsset.path}");
            mediaAsset.SaveToCameraRoll();
            isRecording = false;
        }

        public void StartStreaming()
        {
            if (isStreaming)
            {
                Debug.Log($"StartStreaming: Already streaming");
                return;
            }
            if (isRecording)
            {
                Debug.Log($"StartStreaming: Cannot stream while recording");
                return;
            }
            
            Debug.Log($"StartStreaming()");
            isInitialized = false;
            isStreaming = false;
            
            cameraManager.resolution = VideoKitCameraManager.Resolution._1920x1080;
            cameraManager.frameRate = VideoKitCameraManager.FrameRate._240;
            cameraManager.facing = VideoKitCameraManager.Facing.PreferWorld;
            cameraManager.StartRunning();
        }
        public void StopStreaming()
        {
            if (!isStreaming)
            {
                Debug.Log($"StopStreaming: Not streaming");
                return;
            }
            
            Debug.Log($"StopStreaming()");
            
            cameraManager.StopRunning();
            isStreaming = cameraManager.running;
            
            Debug.Log($"Is streaming: {isStreaming}");
        }
        public void StartRecording()
        {
            if (!isStreaming)
            {
                Debug.Log($"StartRecording: Not streaming");
                return;
            }
            if (isRecording)
            {
                Debug.Log($"StartRecording: Already recording");
                return;
            }
            
            Debug.Log($"StartRecording()");
            cameraRecorder.StartRecording();
            isRecording = cameraRecorder.status == VideoKitRecorder.Status.Recording;
            
            Debug.Log($"Is recording: {isRecording}");
        }
        public void StopRecording()
        {
            if (!isRecording)
            {
                Debug.Log($"StopRecording: Not recording");
                return;
            }
            
            Debug.Log($"StopRecording()");
            cameraRecorder.StopRecording();
            isRecording = cameraRecorder.status != VideoKitRecorder.Status.Recording;
            
            Debug.Log($"Is recording: {isRecording}");
        }
    }
}
