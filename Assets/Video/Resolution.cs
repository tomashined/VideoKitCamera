using System;
using UnityEngine;
using VideoKit;

namespace Video
{
    [Serializable]
    public struct Resolution
    {
        public int Width;
        public int Height;
        public int FrameRate;
        
        public string GetResolution() => $"{Width}x{Height}";
        public Vector2 GetResolutionVector2() => new Vector2(Width, Height);
        public override string ToString() => $"{GetResolution()}@{FrameRate}";
        
        public Resolution(int width, int height, int frameRate)
        {
            Width = width;
            Height = height;
            FrameRate = frameRate;
        }
        public Resolution(string resolution, int frameRate)
        {
            Width = int.Parse(resolution.Split('x')[0]);
            Height = int.Parse(resolution.Split('x')[1]);
            FrameRate = frameRate;
        }
        
        public Resolution(VideoKitCameraManager.Resolution videoKitResolution, VideoKitCameraManager.FrameRate videoKitFrameRate)
        {
            string resolution = videoKitResolution.ToString().Replace("_", "");
            if (resolution.Contains('x'))
            {
                Width = int.Parse(resolution.Split('x')[0]);
                Height = int.Parse(resolution.Split('x')[1]);
            }
            else
            {
                Width = 0;
                Height = 0;
            }

            int.TryParse(videoKitFrameRate.ToString().Replace("_", ""), out FrameRate);
        }
        
        public VideoKitCameraManager.Resolution VideoKitResolution
        {
            get
            {
                Enum.TryParse($"_{Width}x{Height}", out VideoKitCameraManager.Resolution videoKitResolution);
                return videoKitResolution;
            }
        }
        public VideoKitCameraManager.FrameRate VideoKitFrameRate
        {
            get
            {
                Enum.TryParse($"_{FrameRate}", out VideoKitCameraManager.FrameRate videoKitFrameRate);
                return videoKitFrameRate;
            }
        }
    }
}
