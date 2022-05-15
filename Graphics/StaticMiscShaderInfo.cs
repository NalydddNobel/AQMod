using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class StaticMiscShaderInfo<TMiscShaderData> where TMiscShaderData : MiscShaderData
    {
        protected Asset<Effect> effect;
        public Asset<Effect> Effect => effect;

        public TMiscShaderData ShaderData { get => (TMiscShaderData)GameShaders.Misc[ShaderDataKey]; set => GameShaders.Misc[ShaderDataKey] = value; }

        public readonly string ShaderDataKey;

        internal StaticMiscShaderInfo(string requestPath, string key, string pass, Func<Ref<Effect>, string, TMiscShaderData> initalizer)
        {
            effect = ModContent.Request<Effect>("Aequus/Assets/Effects/" + requestPath, 
                (initalizer != null) ? AssetRequestMode.ImmediateLoad : AssetRequestMode.AsyncLoad);
            ShaderDataKey = key;
            if (effect.Value == null)
            {
                throw new Exception(requestPath + " returned null.");
            }
            if (initalizer != null)
            {
                GameShaders.Misc[key] = initalizer(new Ref<Effect>(effect.Value), pass);
            }
        }
    }
    public class StaticMiscShaderInfo : StaticMiscShaderInfo<MiscShaderData>
    {
        internal StaticMiscShaderInfo(string requestPath, string key, string pass, bool loadStatics) : base(requestPath, key, pass, loadStatics ? (effect, pass) => new MiscShaderData(effect, pass) : null)
        {
        }
    }
}