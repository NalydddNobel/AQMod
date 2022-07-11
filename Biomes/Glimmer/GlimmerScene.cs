using Aequus.Common.Utilities;
using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Biomes.Glimmer
{
    public class GlimmerScene : ModSceneEffect
    {
        public static bool RenderedUltimateSword { get; private set; }
        public static Vector2 UltimateSwordWorldDrawLocation;

        public override bool IsSceneEffectActive(Player player)
        {
            return player.InModBiome<GlimmerBiome>();
        }

        public override float GetWeight(Player player)
        {
            return GlimmerSystem.CalcTiles(player) < GlimmerBiome.UltraStariteSpawn ? 1f : 0.5f;
        }

        public static void Draw()
        {
            if (!GlimmerBiome.EventActive)
            {
                RenderedUltimateSword = false;
                UltimateSwordWorldDrawLocation = Vector2.Zero;
                return;
            }
            if (AequusHelpers.debugKey)
                UltimateSwordWorldDrawLocation = AequusHelpers.ScaledMouseworld;
            var gotoPosition = GlimmerBiome.TileLocation.ToWorldCoordinates() + new Vector2(8f, -60f);
            if (!RenderedUltimateSword)
            {
                UltimateSwordWorldDrawLocation = gotoPosition;
            }
            else
            {
                if (UltimateSwordWorldDrawLocation.X > gotoPosition.X)
                {
                    UltimateSwordWorldDrawLocation.X--;
                    if (UltimateSwordWorldDrawLocation.X < gotoPosition.X)
                    {
                        UltimateSwordWorldDrawLocation.X = gotoPosition.X;
                    }
                }
                else if (UltimateSwordWorldDrawLocation.X < gotoPosition.X)
                {
                    UltimateSwordWorldDrawLocation.X++;
                    if (UltimateSwordWorldDrawLocation.X > gotoPosition.X)
                    {
                        UltimateSwordWorldDrawLocation.X = gotoPosition.X;
                    }
                }

                if (UltimateSwordWorldDrawLocation.Y > gotoPosition.Y)
                {
                    UltimateSwordWorldDrawLocation.Y -= 4;
                    if (UltimateSwordWorldDrawLocation.Y < gotoPosition.Y)
                    {
                        UltimateSwordWorldDrawLocation.Y = gotoPosition.Y;
                    }
                }
                else if (UltimateSwordWorldDrawLocation.Y < gotoPosition.Y)
                {
                    UltimateSwordWorldDrawLocation.Y += 4;
                    if (UltimateSwordWorldDrawLocation.Y > gotoPosition.Y)
                    {
                        UltimateSwordWorldDrawLocation.Y = gotoPosition.Y;
                    }
                }
            }
            RenderedUltimateSword = false;
            ScreenCulling.Init(400);
            if (!ScreenCulling.OnScreenWorld(UltimateSwordWorldDrawLocation) && !ScreenCulling.OnScreenWorld(gotoPosition))
            {
                return;
            }

            var drawCoords = UltimateSwordWorldDrawLocation - Main.screenPosition + new Vector2(0f, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 0.5f, -10f, 10f));
            Main.instance.LoadItem(ModContent.ItemType<UltimateSword>());
            var texture = TextureAssets.Item[ModContent.ItemType<UltimateSword>()].Value;
            Main.spriteBatch.Draw(texture, drawCoords, null, Color.White, MathHelper.PiOver4 * 3f, new Vector2(texture.Width, 0f), 1f, SpriteEffects.None, 0f);
            RenderedUltimateSword = true;
        }
    }
}