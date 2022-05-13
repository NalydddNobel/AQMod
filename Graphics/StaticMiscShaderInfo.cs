using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class StaticMiscShaderInfo<T> where T : ShaderData
    {
        private Asset<Effect> effect;
        public Asset<Effect> Effect => effect;

        public MiscShaderData ShaderData { get => GameShaders.Misc[ShaderDataKey]; set => GameShaders.Misc[ShaderDataKey] = value; }

        public readonly string ShaderDataKey;

        internal StaticMiscShaderInfo(string requestPath, string key, string pass, bool loadStatics)
        {
            effect = ModContent.Request<Effect>("Aequus/Assets/Effects/" + requestPath, loadStatics ? AssetRequestMode.ImmediateLoad : AssetRequestMode.AsyncLoad);
            ShaderDataKey = key;
            if (loadStatics)
            {
                GameShaders.Misc[key] = new MiscShaderData(new Ref<Effect>(effect.Value), pass);
            }
            if (effect.Value == null)
            {
                throw new System.Exception(requestPath + " returned null.");
            }
        }
    }
}