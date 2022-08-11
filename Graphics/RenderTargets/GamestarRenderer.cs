using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using ReLogic.Content;

namespace Aequus.Graphics.RenderTargets
{
    public class GamestarRenderer : ScreenTarget
    {
        public class GamestarScreenShaderData : ScreenShaderData
        {
            public GamestarScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
            {
            }

            public override void Apply()
            {
                Main.instance.GraphicsDevice.Textures[1] = Instance.GetTarget();
                base.Apply();
            }
        }

        public static GamestarRenderer Instance { get; private set; }
        public static ParticleRenderer Particles { get; private set; }
        public static List<DrawData> DrawData { get; private set; }

        public const string ScreenShaderKey = "Aequus:Gamestar";

        public GamestarRenderer()
        {
            Particles = new ParticleRenderer();
            DrawData = new List<DrawData>();
        }

        public override void Load(Mod mod)
        {
            base.Load(mod);
            Instance = this;
            var effect = ModContent.Request<Effect>(Aequus.AssetsPath + "Effects/GamestarShader", AssetRequestMode.ImmediateLoad);
            Filters.Scene[ScreenShaderKey] = new Filter(new GamestarScreenShaderData(new Ref<Effect>(effect.Value), "ModdersToolkitShaderPass"), EffectPriority.Low);
        }

        public override void Unload()
        {
            Instance = null;
        }

        protected override bool SelfRequest()
        {
            return Particles.Particles.Count > 0 || DrawData.Count > 0;
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            Begin.GeneralEntities.Begin(Main.spriteBatch);

            Particles.Draw(spriteBatch);
            foreach (var d in DrawData)
                d.Draw(spriteBatch);

            DrawData.Clear();
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
            AequusHelpers.ColorOnlyShader.Apply(null);

            device.SetRenderTarget(_target);
            device.Clear(Color.Transparent);

            spriteBatch.Draw(helperTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

            spriteBatch.End();
            _wasPrepared = true;
        }

        public void DrawOntoScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

            var drawData = new DrawData(GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

            drawData.Draw(spriteBatch);

            spriteBatch.End();
            _wasPrepared = false;
        }
    }
}