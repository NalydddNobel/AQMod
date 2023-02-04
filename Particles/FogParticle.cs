using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace Aequus.Particles
{
    public sealed class FogParticle : BaseParticle<FogParticle>
    {
        public float Light;
        public float calculatingLight;
        public int lightUpdate;

        public override FogParticle CreateInstance()
        {
            return new FogParticle();
        }

        protected override void SetDefaults()
        {
            dontEmitLight = true;
            Light = 0.05f;
            calculatingLight = 0.05f;
            lightUpdate = 0;
            SetTexture(ParticleTextures.fogParticle, 8);
        }

        public void UpdateLighting()
        {
            int size = Math.Max((int)(Scale * 7), 1);
            if (lightUpdate >= size)
            {
                lightUpdate = 0;
                float maxDistance = 900f;
                float distance = MathHelper.Clamp((Main.LocalPlayer.Center - Position).Length(), 0f, maxDistance) / maxDistance;
                Light = MathHelper.Lerp(Light, calculatingLight * (1f - MathF.Pow(distance, 2f)), 0.33f);
                calculatingLight = 0.05f;
                return;
            }
            if (Main.rand.NextBool(Math.Max(Main.frameRate * 10, 2)))
            {
                return;
            }
            var tileCoords = Position.ToTileCoordinates();
            tileCoords.X -= size / 2;
            tileCoords.Y -= size / 2;
            tileCoords.Y += lightUpdate;
            tileCoords.Fluffize(30);
            var lighting = Color.Black;
            for (int i = tileCoords.X; i < tileCoords.X + size; i++)
            {
                var v = Lighting.GetColor(i, tileCoords.Y);
                lighting.R = Math.Max(v.R, lighting.R);
                lighting.G = Math.Max(v.G, lighting.G);
                lighting.B = Math.Max(v.B, lighting.B);
            }
            lightUpdate++;
            calculatingLight = Math.Max((lighting.R + lighting.G + lighting.B) / 765f, calculatingLight);
        }
        public void UpdatePosition()
        {
            var screenPosition = Position - Main.screenPosition;
            float padding = 200f;
            float padding2 = padding - 10f;
            if (screenPosition.X < -padding)
            {
                Position.X += Main.screenWidth + padding2 * 2;
            }
            else if (screenPosition.X > Main.screenWidth + padding)
            {
                Position.X -= Main.screenWidth + padding2 * 2;
            }
            if (screenPosition.Y < -padding)
            {
                Position.Y += Main.screenHeight + padding2 * 2;
            }
            else if (screenPosition.Y > Main.screenHeight + padding)
            {
                Position.Y -= Main.screenHeight + padding2 * 2;
            }
        }
        public override void Update(ref ParticleRendererSettings settings)
        {
            UpdatePosition();
            UpdateLighting();
            Velocity = Velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
            if (Velocity == Vector2.Zero)
            {
                Scale -= Main.rand.NextFloat(0.001f, 0.005f);
            }
            else
            {
                Scale += 0.004f;
                if (Scale > 1f)
                {
                    Velocity *= Main.rand.NextFloat(0.99f, 1f);
                    if (Velocity.Length() < 0.03f || Scale > 1.75f)
                    {
                        Velocity = Vector2.Zero;
                    }
                }
            }
            if (Scale <= 0.001f || float.IsNaN(Scale))
            {
                RestInPool();
                return;
            }
            Position += Velocity;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color * (1f - Light) * MathF.Pow(Scale, 2f), Rotation, origin, Scale * 2.33f, SpriteEffects.None, 0f);
        }
    }
}