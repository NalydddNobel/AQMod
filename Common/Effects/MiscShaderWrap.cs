using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Common.Effects
{
    public class MiscShaderWrap<T> where T : MiscShaderData
    {
        protected Asset<Effect> effect;
        public Asset<Effect> Effect => effect;

        public T ShaderData { get => (T)GameShaders.Misc[Key]; set => GameShaders.Misc[Key] = value; }

        public readonly string Key;

        internal MiscShaderWrap(string requestPath, string key, string pass, Func<Ref<Effect>, string, T> initalizer)
        {
            key = string.Join(':', "Aequus", key);
            effect = ModContent.Request<Effect>(requestPath,
                initalizer != null ? AssetRequestMode.ImmediateLoad : AssetRequestMode.AsyncLoad);
            Key = key;
            if (initalizer != null)
            {
                if (effect.Value == null)
                {
                    throw new Exception(requestPath + " returned null.");
                }
                GameShaders.Misc[key] = initalizer(new Ref<Effect>(effect.Value), pass);
            }
        }

        public MiscShaderWrap<T> UseColor(Vector3 color)
        {
            ShaderData.UseColor(color);
            return this;
        }
        public MiscShaderWrap<T> UseImage1(Asset<Texture2D> texture)
        {
            ShaderData.UseImage1(texture);
            return this;
        }

        public void Apply(DrawData? drawData)
        {
            ShaderData.Apply(drawData);
        }

        public static implicit operator T(MiscShaderWrap<T> wrap)
        {
            return wrap.ShaderData;
        }
    }

    public class MiscShaderWrap : MiscShaderWrap<MiscShaderData>
    {
        internal MiscShaderWrap(string requestPath, string key, string pass, bool loadStatics)
            : base(requestPath,
                  key,
                  pass,
                  loadStatics ? (effect, pass) => new MiscShaderData(effect, pass) : null
              )
        {
        }
    }
}