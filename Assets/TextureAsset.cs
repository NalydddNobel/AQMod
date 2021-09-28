using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public sealed class TextureAsset : AssetItem<Texture2D>
    {
        private string _path;
        private const string throwbackTexturePath = "AQMod/Assets/Textures/error";
        private static bool _debug = true;

        internal TextureAsset(string path)
        {
            _path = path;
        }

        internal static TextureAsset FromT<T>(string extra)
        {
            return FromType(typeof(T), extra);
        }

        internal static TextureAsset FromType(Type t, string extra)
        {
            var textureAsset = new TextureAsset(t.Namespace.Replace(".", "/") + "/" + t.Name + extra);
            if (_debug)
                AQMod.Instance.Logger.Debug("Creating asset: " + textureAsset._path);
            return textureAsset;
        }

        public override void LoadAsset()
        {
            if (!_loaded)
            {
                _asset = new Ref<Texture2D>(ModContent.TextureExists(_path) ? ModContent.GetTexture(_path) : ModContent.GetTexture(throwbackTexturePath));
            }
        }
    }
}