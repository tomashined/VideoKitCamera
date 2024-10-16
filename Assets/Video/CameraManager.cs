using UnityEngine;

namespace Video
{
    public class CameraManager : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private bool isStreaming;
        [SerializeField] private bool isRecording;
        
        public bool IsStreaming => isStreaming;
        public bool IsRecording => isRecording;
        
        public void StartStreaming()
        {
            
        }
        public void StopStreaming()
        {
            
        }
        public void StartRecording()
        {
            
        }
        public void StopRecording()
        {
            
        }
    }
}
