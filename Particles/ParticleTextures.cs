using Aequus.Common;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Particles
{
    public class ParticleTextures : ILoadable
    {
        public static SpriteInfo fogParticle;
        public static SpriteInfo fogParticleHQ;

        internal static SpriteInfo monoParticle;
        public static SpriteInfo MonoParticle => monoParticle;

        internal static SpriteInfo gamestarParticle;
        public static SpriteInfo GamestarParticle => gamestarParticle;

        internal static SpriteInfo shinyFlashParticle;
        public static SpriteInfo ShinyFlashParticle => shinyFlashParticle;

        void ILoadable.Load(Mod mod)
        {
            if (!Main.dedServ)
            {
                fogParticleHQ = new SpriteInfo(ModContent.Request<Texture2D>(AequusHelpers.GetPath<FogParticle>(), AssetRequestMode.ImmediateLoad), verticalFrames: 8, originFracX: 0.5f, originFracY: 0.5f);
                fogParticle = new SpriteInfo(ModContent.Request<Texture2D>(AequusHelpers.GetPath<FogParticle>(), AssetRequestMode.ImmediateLoad), verticalFrames: 8, originFracX: 0.5f, originFracY: 0.5f);
                monoParticle = new SpriteInfo(ModContent.Request<Texture2D>($"{Aequus.AssetsPath}Particles/Particle", AssetRequestMode.ImmediateLoad), 1, 3, 0.5f, 0.5f);
                gamestarParticle = new SpriteInfo(ModContent.Request<Texture2D>($"{Aequus.AssetsPath}Particles/GamestarParticle", AssetRequestMode.ImmediateLoad), 1, 1, 0.5f, 0.5f);
                shinyFlashParticle = new SpriteInfo(ModContent.Request<Texture2D>($"{AequusHelpers.GetPath<ShinyFlashParticle>()}", AssetRequestMode.ImmediateLoad), 1, 1, 0.5f, 0.5f);
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
