using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Common.Graphics {
    public class TrailTextures : ILoadable
    {
        [AttributeUsage(AttributeTargets.Property)]
        private sealed class ImgPathAttribute : Attribute
        {
            public readonly string Path;

            public ImgPathAttribute(string path)
            {
                Path = path;
            }
        }
        [AttributeUsage(AttributeTargets.Property)]
        private sealed class ImgArrAttribute : Attribute
        {
            public readonly int Length;

            public ImgArrAttribute(int length)
            {
                Length = length;
            }
        }

        private static BindingFlags SearchFlags => BindingFlags.Public | BindingFlags.Static;

        [ImgPath("Assets/Effects/Prims")]
        [ImgArr(5)]
        public static Asset<Texture2D>[] Trail { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            foreach (var p in typeof(TrailTextures).GetProperties(SearchFlags))
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
            foreach (var p in typeof(TrailTextures).GetProperties(SearchFlags))
            {
                p.SetValue(null, null);
            }
        }
    }
}