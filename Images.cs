using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus
{
    public sealed class Images : ILoadable
    {
        private sealed class ImgPathAttribute : Attribute
        {
            public readonly string Path;

            public ImgPathAttribute(string path)
            {
                Path = path;
            }
        }
        private sealed class ImgArrAttribute : Attribute
        {
            public readonly int Length;

            public ImgArrAttribute(int length)
            {
                Length = length;
            }
        }

        private static BindingFlags SearchFlags => BindingFlags.Public | BindingFlags.Static;

        public static Asset<Texture2D> Pixel { get; private set; }
        [ImgPath("Assets/Particles")]
        public static Asset<Texture2D> Particle { get; private set; }

        public static Asset<Texture2D> LightRay { get; private set; }
        public static Asset<Texture2D> TextBloom { get; private set; }
        [ImgPath("Assets/UI")]
        public static Asset<Texture2D> StatusBubble { get; private set; }
        public const int StatusBubbleFrames = 2;
        [ImgPath("UI")]
        public static Asset<Texture2D> InventoryBack { get; private set; }

        [ImgArr(5)]
        public static Asset<Texture2D>[] Bloom { get; private set; }
        [ImgPath("Assets/Effects/Prims")]
        [ImgArr(4)]
        public static Asset<Texture2D>[] Trail { get; private set; }
        [ImgArr(2)]
        public static Asset<Texture2D>[] Explosion { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            foreach (var p in typeof(Images).GetProperties(SearchFlags))
            {
                if (p.PropertyType == typeof(Asset<Texture2D>[]))
                {
                    InnerLoadArrayProperty(p);
                }
                else if (p.PropertyType == typeof(Asset<Texture2D>))
                {
                    InnerLoadProperty(p);
                }
            }
        }
        private void InnerLoadArrayProperty(PropertyInfo p)
        {
            string folderSpace = InnerGetPath(p);
            int arrLength = p.GetCustomAttribute<ImgArrAttribute>().Length;
            var arr = new Asset<Texture2D>[arrLength];
            for (int i = 0; i < arrLength; i++)
            {
                arr[i] = ModContent.Request<Texture2D>("Aequus/" + folderSpace + "/" + p.Name + i);
            }
            p.SetMethod.Invoke(null, new object[] { arr });
        }
        private void InnerLoadProperty(PropertyInfo p)
        {
            string folderSpace = InnerGetPath(p);
            p.SetMethod.Invoke(null, new object[] { ModContent.Request<Texture2D>("Aequus/" + folderSpace + "/" + p.Name) });
        }
        private string InnerGetPath(PropertyInfo p)
        {
            string folderSpace = "Assets";
            var attr = p.GetCustomAttribute<ImgPathAttribute>();
            if (attr != null)
            {
                folderSpace = attr.Path;
            }
            return folderSpace;
        }

        void ILoadable.Unload()
        {
            foreach (var p in typeof(Images).GetProperties(SearchFlags))
            {
                p.SetValue(null, null);
            }
        }
    }
}