using Aequus.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace Aequus.Common.Graphics {
    public partial class AequusDrawing {
        public static Entity DrawnEntity;

        private record struct VanillaHealthbarDrawInfo(float X, float Y, int Health, int MaxHealth, float alpha, float scale, bool noFlip);

        private void Load_HealthbarDrawing() {
            On_Main.DrawHealthBar += On_Main_DrawHealthBar;
        }

        private static void On_Main_DrawHealthBar(On_Main.orig_DrawHealthBar orig, Main self, float X, float Y, int Health, int MaxHealth, float alpha, float scale, bool noFlip) {
            orig(self, X, Y, Health, MaxHealth, alpha, scale, noFlip);
            if (DrawnEntity is NPC npc && npc.TryGetAequus(out var aequusNPC)) {
                int healthbarsDrawn = 0;
                VanillaHealthbarDrawInfo info = new(X, Y, Health, MaxHealth, alpha, scale, noFlip);
                if (aequusNPC.soulHealthTotal > 0) {
                    float wave = Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.5f, 1f);
                    DrawHealthbarOverlay(healthbarsDrawn, info, AequusTextures.GenericOverlay, Color.Red.HueSet(0.5f + wave * 0.1f) * wave, npc, aequusNPC, aequusNPC.soulHealthTotal, npc.lifeMax);
                    healthbarsDrawn++;
                }
            }
            DrawnEntity = null;
        }

        private static void DrawHealthbarOverlay(int healthbarsDrawn, VanillaHealthbarDrawInfo healthbarDrawInfo, Texture2D healthbarTexture, Color drawColor, NPC npc, AequusNPC aequusNPC, int health, int healthMaximum) {
            if (health <= 0) {
                return;
            }

            float healthRatio = MathHelper.Clamp(health / (float)healthMaximum, 0f, 1f);
            var frame = healthbarTexture.Bounds;
            int healthFrameX = (int)(frame.Width * healthRatio);
            frame.X += frame.Width - healthFrameX;
            frame.Width = healthFrameX;
            float drawX = healthbarDrawInfo.X + (-TextureAssets.Hb1.Value.Width / 2f + frame.X) * healthbarDrawInfo.scale;
            float drawY = healthbarDrawInfo.Y;
            Main.spriteBatch.Draw(
                healthbarTexture,
                new Vector2(drawX, drawY) - Main.screenPosition,
                frame,
                drawColor,
                0f,
                Vector2.Zero,
                healthbarDrawInfo.scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}