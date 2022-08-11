using Aequus.Graphics.RenderTargets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace Aequus.Particles
{
    public class SquareParticle : MonoParticle
    {
        private static Texture2D texture;
        public static Asset<Texture2D> SquareParticleTexture { get; private set; }
        public static Rectangle Frame { get; private set; }
        public static Vector2 Origin { get; private set; }

        private class Loader : ILoadable
        {
            void ILoadable.Load(Mod mod)
            {
                SquareParticleTexture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "Particles/SquareParticle", AssetRequestMode.ImmediateLoad);
                texture = SquareParticleTexture.Value;
                Frame = SquareParticleTexture.Frame();
                Origin = Frame.Size() / 2f;
            }

            void ILoadable.Unload()
            {
                SquareParticleTexture = null;
                texture = null;
            }
        }

        public SquareParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1, float rotation = 0) : base(position, velocity, color, scale, rotation)
        {
        }

        public override Texture2D Texture => texture;

        public override void OnAdd()
        {
            frame = Frame;
            origin = Origin;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            if (Color.A == 0)
            {
                Scale = 0f;
            }
            float s = Scale;
            base.Update(ref settings);
            Scale = (int)s;
            while (Scale > 0f)
            {
                if (Main.rand.NextBool(Math.Max(10 - (int)Scale / 2, 2)))
                {
                    Scale--;
                    continue;
                }
                break;
            }
            if (Scale > 4f && Main.rand.NextBool(35 + Main.rand.Next(15 + (int)Scale * 5)))
            {
                GamestarRenderer.Particles.Add(new SquareParticle(new Vector2(Position.X + Main.rand.NextFloat(-2f, 2f) * Scale, Position.Y + Main.rand.NextFloat(-2f, 2f) * Scale),
                    Main.rand.NextVector2Unit() * Velocity.Length(), Color, Scale - Main.rand.Next((int)Scale - 4)));
            }
            Rotation = 0f;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            var drawCoords = new Vector2((int)(Position.X / 2f) * 2f, (int)(Position.Y / 2f) * 2f) - Main.screenPosition;
            spritebatch.Draw(Texture, drawCoords, frame, ParticleColor, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}