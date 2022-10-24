using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public class OrganicEnergyEventLightingSystem : ModSystem
    {
        public static Color CurrentColor;
        public static int SparkleChance;

        public override void Load()
        {
            On.Terraria.Graphics.Light.TileLightScanner.GetTileLight += TileLightScanner_GetTileLight;
        }

        public override void PreUpdateEntities()
        {
            if (CurrentColor.R > 0 || CurrentColor.G > 0 || CurrentColor.B > 0)
            {
                CurrentColor *= 0.99f;
                CurrentColor.A = 255;
                SparkleChance = (int)(SparkleChance * 0.99f);
            }
            else
            {
                CurrentColor.A = 0;
                SparkleChance = 0;
            }
        }

        public static void TileLightScanner_GetTileLight(On.Terraria.Graphics.Light.TileLightScanner.orig_GetTileLight orig, TileLightScanner self, int x, int y, out Vector3 outputColor)
        {
            if (CurrentColor.A == 255)
            {
                int numBlockingLight = 0;
                for (int i = -1; i <= 1; i += 2)
                {
                    for (int j = -1; j <= 1; j += 2)
                    {
                        if (Main.tile[x + i, y + j].HasUnactuatedTile && Main.tileBlockLight[Main.tile[x + i, y + j].TileType])
                        {
                            numBlockingLight++;
                        }
                    }
                }
                if (numBlockingLight >= 4)
                    goto Orig;

                if (SparkleChance > 0 && (!Main.tile[x, y].HasUnactuatedTile || !Main.tileSolid[Main.tile[x, y].TileType]) && Aequus.GameWorldActive && Main.rand.NextBool(SparkleChance))
                {
                    if (Main.tile[x, y + 1].HasUnactuatedTile && Main.tileBlockLight[Main.tile[x, y + 1].TileType] && Main.tileSolid[Main.tile[x, y + 1].TileType])
                    {
                        var d = Dust.NewDustDirect(new Vector2(x * 16f, y * 16f + 16f), 16, 2, DustID.TintableDustLighted, 0f, Main.rand.NextFloat(-6f, -2f), 128, CurrentColor.UseA(0), Main.rand.NextFloat(0.25f, 0.75f));
                        d.velocity.X *= 0.2f;
                        d.velocity *= d.scale;
                        d.fadeIn = d.scale + 0.5f;
                        d.rotation = 0f;
                        d.noGravity = true;
                        d.noLightEmittence = true;
                    }
                }
                Lighting.AddLight(new Vector2(x, y) * 16f, CurrentColor.ToVector3() / Lighting.GlobalBrightness);
                orig(self, x, y, out outputColor);
                return;
            }

        Orig:
            orig(self, x, y, out outputColor);
        }
    }
}