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

        [Header("Time")]
        [SerializeField] private float durationSeconds;
        [SerializeField] private float durationMs;
        [SerializeField] private long timestampNs;
        [SerializeField] private float timestampMs;

        [Header("State")]
        [SerializeField] private long startTimestampNs;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float frameRate;
        [SerializeField] private Texture2D texture;

        private List<string> timestamps;
        
        private async Task LoadVideo(Action<Texture2D> onSetTexture)
        {
            Debug.Log($"Replaying video: {videoPath}");
            if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath)) return;
            mediaAsset = await MediaAsset.FromFile(videoPath);
            if (mediaAsset == null) return;
            
            width = mediaAsset.width;
            height = mediaAsset.height;
            frameRate = mediaAsset.frameRate;
            durationSeconds = mediaAsset.duration;
            durationMs = durationSeconds * 1000;
            Debug.Log($"{width}x{height} @{frameRate}Hz {mediaAsset.duration}s");
            
            // Create and display texture
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            onSetTexture?.Invoke(texture);
        }
        
        public async void OnReplayButtonPressed(Action<Texture2D> onSetTexture)
        {
            await LoadVideo(onSetTexture);
            
            timestamps = new List<string>();
            // Read pixel buffers
            foreach (var pixelBuffer in mediaAsset.Read<PixelBuffer>()) {
                // Copy pixel data into the texture
                var textureBuffer = new PixelBuffer(texture);
                pixelBuffer.CopyTo(textureBuffer);
                timestampNs = pixelBuffer.timestamp;
                timestamps.Add($"{timestampNs}");
                // Upload the texture data to the GPU
                texture.Apply();
                // Wait for next frame
                await Task.Yield();
            }
            // Log
            Debug.Log($"Finished reading frames");
            await File.WriteAllLinesAsync(Path.Combine(Application.persistentDataPath, "timestamps.txt"), timestamps);
        }
        public async void OnFindButtonPressed(Action<Texture2D> onSetTexture)
        {
            await LoadVideo(onSetTexture);
            
            timestampMs = Mathf.Clamp(timestampMs, 0, durationMs);
            timestampNs = (long)(timestampMs * 1000000f);
            await Task.Yield();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var pixelBuffer in mediaAsset.Read<PixelBuffer>())
            {
                if (pixelBuffer.timestamp >= timestampNs)
                {
                    var textureBuffer = new PixelBuffer(texture);
                    pixelBuffer.CopyTo(textureBuffer);
                    timestampNs = pixelBuffer.timestamp;
                    // Upload the texture data to the GPU
                    texture.Apply();
                    
                    stopwatch.Stop();
                    Debug.Log($"Frame at {timestampMs}ms is {timestampNs} after {stopwatch.ElapsedMilliseconds}ms");
                    
                    break;
                }
            }
        }
        public async void OnFindButtonPressedAndRecord(Action<Texture2D> onSetTexture)
        {
            await LoadVideo(onSetTexture);
            
            timestampMs = Mathf.Clamp(timestampMs, 0, durationMs);
            timestampNs = (long)(timestampMs * 1000000f);
            await Task.Yield();
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            var recorder = await MediaRecorder.Create(
                format: MediaRecorder.Format.MP4,
                width: width,
                height: height,
                frameRate: frameRate
                );
            startTimestampNs = -1;
            foreach (var pixelBuffer in mediaAsset.Read<PixelBuffer>())
            {
                if (pixelBuffer.timestamp >= timestampNs)
                {
                    var textureBuffer = new PixelBuffer(texture);
                    pixelBuffer.CopyTo(textureBuffer);
                    timestampNs = pixelBuffer.timestamp;
                    // Upload the texture data to the GPU
                    texture.Apply();
                    if (startTimestampNs < 0) startTimestampNs = timestampNs;
                    using var newpixelBuffer = new PixelBuffer(
                        texture,            // texture to get pixel data from
                        timestampNs - startTimestampNs   // pixel buffer timestamp to use for recording
                        );
                    
                    stopwatch.Stop();
                    //Debug.Log($"Frame at {timestampMs}ms is {timestampNs} after {stopwatch.ElapsedMilliseconds}ms");
                    
                    recorder.Append(newpixelBuffer);
                }
            }
            MediaAsset asset = await recorder.FinishWriting();
            Debug.Log($"Recorded {asset.width}x{asset.height}@{asset.frameRate} - {asset.path}");
        }
        
        public async void OnFindButtonPressed1(Action<Texture2D> onSetTexture)
        {
            await LoadVideo(onSetTexture);
            
            timestampMs = Mathf.Clamp(timestampMs, 0, durationMs);
            timestampNs = (long)(timestampMs * 1000000f);
            await Task.Yield();
            
            var pixelBuffer = mediaAsset.Read<PixelBuffer>().ToList().Find(buffer => buffer.timestamp >= timestampNs);
            var textureBuffer = new PixelBuffer(texture);
            pixelBuffer.CopyTo(textureBuffer);
            timestampNs = pixelBuffer.timestamp;
            // Upload the texture data to the GPU
            texture.Apply();
        }
        public void OnFindFieldChanged(int evtNewValue)
        {
            timestampMs = evtNewValue;
        }
    }
}
