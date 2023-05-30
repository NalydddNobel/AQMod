using Aequus.Common.Effects;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Common.Rendering {
    public class RadonMossFogRenderer : ScreenTarget
    {
        public static RadonMossFogRenderer Instance { get; private set; }
        public static LegacyMiscShaderWrap Shader { get; private set; }
        public static List<Point> Tiles { get; private set; }
        public static List<DrawInfo> DrawInfoCache { get; private set; }
        public struct DrawInfo
        {
            public readonly Vector2 Position;
            public readonly float Intensity;
            public readonly Rectangle Frame;
            public readonly float Rotation;

            public DrawInfo(Vector2 pos, float intensity, Rectangle frame, float rotation)
            {
                Position = pos;
                Intensity = intensity;
                Frame = frame;
                Rotation = rotation;
            }
        }

        public override int FinalResultResolutionDiv => 2;

        public override void Load(Mod mod)
        {
            base.Load(mod);
            Tiles = new List<Point>();
            DrawInfoCache = new List<DrawInfo>();
            Instance = this;
            if (!Main.dedServ)
            {
                Shader = new LegacyMiscShaderWrap("Aequus/Assets/Effects/RadonMossShader", "Aequus:RadonMossFog", "RadonShaderPass", loadStatics: true);
            }
        }

        public override void Unload()
        {
            Shader = null;
            Instance = null;
            DrawInfoCache?.Clear();
            DrawInfoCache = null;
            Tiles?.Clear();
            Tiles = null;
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            //Main.NewText(Tiles.Count);
            //Main.NewText(DrawInfoCache.Count);
            if (Tiles.Count > 0)
            {
                DrawInfoCache.Clear();
                foreach (var p in Tiles)
                {
                    int i = p.X;
                    int j = p.Y;
                    var rand = new FastRandom(i * i + j * j * i);
                    var lighting = Helper.GetBrightestLight(new Point(i, j), 6);
                    float intensity = 1f - (lighting.R + lighting.G + lighting.B) / 765f;
                    intensity = MathHelper.Lerp(intensity, 1f, (float)MathHelper.Clamp(Vector2.Distance(new Vector2(i * 16f + 8f, j * 16f + 8f), Main.LocalPlayer.Center) / 300f - MathF.Sin(Main.GlobalTimeWrappedHourly * rand.Float(0.1f, 0.6f)).Abs(), 0f, 1f));
                    if (intensity <= 0f)
                        continue;
                    DrawInfoCache.Add(new DrawInfo(new Vector2(i * 16f, j * 16f) + new Vector2(8f).RotatedBy(rand.Float(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * rand.Float(0.3f, 0.6f)), intensity, ParticleTextures.fogParticle.Frame.Frame(0, frameY: rand.Next(ParticleTextures.fogParticle.FramesY)), rand.Next(4) * MathHelper.PiOver2));
                }
                Tiles.Clear();
            }
            if (DrawInfoCache.Count > 0)
            {
                Main.spriteBatch.Begin_World(shader: false); ;

                var texture = ParticleTextures.fogParticle.Texture.Value;
                var origin = ParticleTextures.fogParticle.Origin;
                foreach (var info in DrawInfoCache)
                {
                    Main.spriteBatch.Draw(texture, info.Position - Main.screenPosition, info.Frame, Color.Black * info.Intensity, info.Rotation, origin, 4f, SpriteEffects.None, 0f);
                }

                PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth / FinalResultResolutionDiv, Main.screenHeight / FinalResultResolutionDiv, RenderTargetUsage.PreserveContents);

                spriteBatch.End();

                spriteBatch.Begin();
                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);

                spriteBatch.Draw(helperTarget, new Rectangle(0, 0, Main.screenWidth / FinalResultResolutionDiv, Main.screenHeight / FinalResultResolutionDiv), Color.White);

                spriteBatch.End();

                _wasPrepared = true;
            }
        }

        protected override bool PrepareTarget()
        {
            return Tiles.Count > 0 || DrawInfoCache.Count > 0;
        }

        public void DrawOntoScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);

            var drawData = new DrawData(GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(255, 255, 255, 255));
            //Shader.ShaderData.Apply(drawData);

            drawData.Draw(spriteBatch);

            spriteBatch.End();
            _wasPrepared = false;
        }
    }
}
