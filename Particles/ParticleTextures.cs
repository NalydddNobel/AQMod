using Aequus.Graphics;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Particles
{
    public class ParticleTextures : ILoadable
    {
        public static TextureInfo fogParticle;
        public static TextureInfo fogParticleHQ;

        internal static TextureInfo monoParticle;
        public static TextureInfo MonoParticle => monoParticle;

        internal static TextureInfo gamestarParticle;
        public static TextureInfo GamestarParticle => gamestarParticle;

        internal static TextureInfo shinyFlashParticle;
        public static TextureInfo ShinyFlashParticle => shinyFlashParticle;

        void ILoadable.Load(Mod mod)
        {
            if (!Main.dedServ)
            {
                fogParticleHQ = new TextureInfo(ModContent.Request<Texture2D>(AequusHelpers.GetPath<FogParticle>(), AssetRequestMode.ImmediateLoad), verticalFrames: 8, originFracX: 0.5f, originFracY: 0.5f);
                fogParticle = new TextureInfo(ModContent.Request<Texture2D>(AequusHelpers.GetPath<FogParticle>(), AssetRequestMode.ImmediateLoad), verticalFrames: 8, originFracX: 0.5f, originFracY: 0.5f);
                monoParticle = new TextureInfo(ModContent.Request<Texture2D>($"{Aequus.AssetsPath}Particles/Particle", AssetRequestMode.ImmediateLoad), 1, 3, 0.5f, 0.5f);
                gamestarParticle = new TextureInfo(ModContent.Request<Texture2D>($"{Aequus.AssetsPath}Particles/GamestarParticle", AssetRequestMode.ImmediateLoad), 1, 1, 0.5f, 0.5f);
                shinyFlashParticle = new TextureInfo(ModContent.Request<Texture2D>($"{AequusHelpers.GetPath<ShinyFlashParticle>()}", AssetRequestMode.ImmediateLoad), 1, 1, 0.5f, 0.5f);
            }
        }

        void ILoadable.Unload()
        {
            fogParticleHQ = null;
            fogParticle = null;
            shinyFlashParticle = null;
            monoParticle = null;
            gamestarParticle = null;
        }
    }
}
