using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
namespace Video
{
    [Serializable]
    public class VideoData
    {
        [Header("Video")]
        [SerializeField] private string name;
        [SerializeField] private Resolution resolution;
        [SerializeField] private Texture2D texture;
        
        public string Name
        {
            get => name;
            set
            {
                name = value;
                infoBinding?.MarkDirty();
            }
        }
        public Resolution Resolution
        {
            get => resolution;
            set
            {
                resolution = value;
                infoBinding?.MarkDirty();
            }
        }
        [CreateProperty] public string Info => $"{name} [{resolution.ToString()}]";
        [CreateProperty] public Texture2D Texture
        {
            get => texture;
            set
            {
                texture = value;
                textureBinding?.MarkDirty();
            }
        }
        
        private DataBinding infoBinding;
        public DataBinding InfoBinding => infoBinding ??= new DataBinding {
            dataSourcePath = new PropertyPath(nameof(Info)),
            updateTrigger = BindingUpdateTrigger.WhenDirty,
            bindingMode = BindingMode.ToTarget,
        };
        private DataBinding textureBinding;
        public DataBinding TextureBinding => textureBinding ??= new DataBinding {
            dataSourcePath = new PropertyPath(nameof(Texture)),
            updateTrigger = BindingUpdateTrigger.WhenDirty,
            bindingMode = BindingMode.ToTarget,
        };
    }
}
