using Aequus.Content.Biomes.PollutedOcean;
using System;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Light;

namespace Aequus.Core.Graphics;

[Autoload(Side = ModSide.Client)]
public class CavernsLightEmitter : ModSystem {
    public static Vector3 CavernLight { get; set; }

    private static bool _appliedHook;

    public override void PreUpdateEntities() {
        if (Main.LocalPlayer.InModBiome<PollutedOceanBiomeUnderground>()) {
            CavernLight = PollutedOceanBiomeUnderground.CavernLight * 0.1f;
        }
        else if (CavernLight != Vector3.Zero) {
            CavernLight *= 0.9f;
            if (CavernLight.LengthSquared() < 1f) {
                CavernLight = Vector3.Zero;
                if (_appliedHook) {
                    On_TileLightScanner.ApplyWallLight -= TemporaryLightHook;
                    _appliedHook = false;
                }
            }
        }

        if (CavernLight != Vector3.Zero && !_appliedHook) {
            On_TileLightScanner.ApplyWallLight += TemporaryLightHook;
            _appliedHook = true;
        }
    }

    private static void TemporaryLightHook(On_TileLightScanner.orig_ApplyWallLight orig, TileLightScanner self, Tile tile, int x, int y, ref Terraria.Utilities.FastRandom localRandom, ref Vector3 lightColor) {
        orig(self, tile, x, y, ref localRandom, ref lightColor);
        if (y > Main.worldSurface && y < Main.UnderworldLayer && BlockAllowsLight(tile, x, y) && WallAllowsLight(tile)) {
            Vector3 cavernLight = CavernLight;
            lightColor.X = Math.Max(lightColor.X, cavernLight.X);
            lightColor.Y = Math.Max(lightColor.Y, cavernLight.Y);
            lightColor.Z = Math.Max(lightColor.Z, cavernLight.Z);
        }
    }

    private static bool WallAllowsLight(Tile tile) {
        return Main.wallLight[tile.WallType] || (!TileHelper.ShowEcho && (tile.IsWallInvisible || tile.WallType == WallID.EchoWall));
    }

    private static bool BlockAllowsLight(Tile tile, int x, int y) {
        return (!tile.HasTile || !Main.tileNoSunLight[tile.TileType] || tile.Slope != 0 || tile.IsHalfBlock || !TileDrawing.IsVisible(tile)) && !BlockAllowsLight_CheckWater(x, y);
    }

    private static bool BlockAllowsLight_CheckWater(int x, int y) {
        return Main.tile[x, y].LiquidAmount != 0 || Main.tile[x, y - 1].LiquidAmount != 0 || Main.tile[x, y + 1].LiquidAmount != 0 || Main.tile[x - 1, y].LiquidAmount != 0 || Main.tile[x + 1, y].LiquidAmount != 0;
    }
}