using Aequus.NPCs.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Aequus.Biomes.Glimmer
{
    public class GlimmerSky : CustomSky
    {
        public const string Key = "Aequus:GlimmerEventSky";

        public Asset<Texture2D> skyTexture;

        public bool active;
        public float opacityBasedOnDistance;

        public override void Update(GameTime gameTime)
        {
            if (active)
            {
                if (Opacity < 1f)
                {
                    Opacity = Math.Min(Opacity + 0.02f, 1f);
                }
            }
            else
            {
                if (Opacity > 0f)
                {
                    Opacity = Math.Max(Opacity - 0.02f, 0f);
                }
            }
            float wantedOpacity;
            if (GlimmerBiome.omegaStarite != -1)
            {
                wantedOpacity = Opacity;
            }
            else
            {
                float distance = 1f - GlimmerSystem.CalcTiles(Main.LocalPlayer) / (float)GlimmerBiome.MaxTiles;
                wantedOpacity = Opacity * Math.Max(distance, 0f);
            }
            opacityBasedOnDistance = MathHelper.Lerp(opacityBasedOnDistance, wantedOpacity, 0.05f);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            int y = (int)(-Main.screenPosition.Y / (Main.worldSurface * 16.0 - 600.0) * 200.0);
            var destinationRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            destinationRectangle.Height -= y;
            Color clr = Color.White * opacityBasedOnDistance;
            if (maxDepth == float.MaxValue && minDepth != float.MaxValue)
            {
                spriteBatch.Draw(skyTexture.Value, destinationRectangle, clr.UseA(0));
                return;
            }

            if (minDepth > 10f)
                return;

            if (minDepth == float.MinValue)
            {
                minDepth = 0f;
            }

            float approxProgress = Math.Max(minDepth / 10f, 0.1f);
            destinationRectangle.Y += (int)(y * approxProgress + AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 0.1f + approxProgress * MathHelper.TwoPi, -40f, 360f));

            spriteBatch.Draw(skyTexture.Value, destinationRectangle, Color.Lerp(clr.UseA(0), Color.Blue * 0.01f, 1f - approxProgress));
        }

        public override bool IsActive()
        {
            return active || Opacity > 0f || NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
        }

        public override float GetCloudAlpha()
        {
            return 1f - opacityBasedOnDistance * 0.75f;
        }

        public override void Reset()
        {
        }

        public override void OnLoad()
        {
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            active = true;
            if (skyTexture == null)
            {
                skyTexture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "GlimmerSky");
            }
        }

        public override void Deactivate(params object[] args)
        {
            active = false;
        }
    }
}