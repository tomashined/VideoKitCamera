using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VideoKit;
namespace Video
{
    public class ReplayManager : MonoBehaviour
    {
        private MediaAsset mediaAsset;
        
        [Header("Parameters")]
        [SerializeField] private string videoPath;
        [SerializeField] private int timestampMs;

        [Header("State")]
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float frameRate;
        [SerializeField] private int durationMs;
        [SerializeField] private Texture2D texture;
        
        private async Task LoadVideo(Action<Texture2D> onSetTexture)
        {
            Debug.Log($"Replaying video: {videoPath}");
            if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath)) return;
            mediaAsset = await MediaAsset.FromFile(videoPath);
            if (mediaAsset == null) return;
            
            width = mediaAsset.width;
            height = mediaAsset.height;
            frameRate = mediaAsset.frameRate;
            durationMs = (int)(mediaAsset.duration * 1000);
            Debug.Log($"{width}x{height} @{frameRate}Hz {mediaAsset.duration}s");
            
            // Create and display texture
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            onSetTexture?.Invoke(texture);
        }
        
        public async void OnReplayButtonPressed(Action<Texture2D> onSetTexture)
        {
            await LoadVideo(onSetTexture);
            
            // Read pixel buffers
            foreach (var pixelBuffer in mediaAsset.Read<PixelBuffer>()) {
                // Copy pixel data into the texture
                var textureBuffer = new PixelBuffer(texture);
                pixelBuffer.CopyTo(textureBuffer);
                timestampMs = (int)pixelBuffer.timestamp;
                // Upload the texture data to the GPU
                texture.Apply();
                // Wait for next frame
                await Task.Yield();
            }
            // Log
            Debug.Log($"Finished reading frames");
        }
        public async void OnFindButtonPressed(Action<Texture2D> onSetTexture)
        {
            await LoadVideo(onSetTexture);
            
            timestampMs = Mathf.Clamp(timestampMs, 0, durationMs);
            
            var textureBuffer = new PixelBuffer(texture);
            var pixelBuffer = mediaAsset.Read<PixelBuffer>().First(buffer => buffer.timestamp >= timestampMs);
            Debug.Log($"Frame at {timestampMs} is {pixelBuffer.timestamp}");
            pixelBuffer.CopyTo(textureBuffer);
            texture.Apply();
        }
        public void OnFindFieldChanged(int evtNewValue)
        {
            timestampMs = evtNewValue;
        }
    }
}
