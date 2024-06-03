using Aequus.Content.Biomes.PollutedOcean;
using System;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Light;

namespace Aequus.Core.Graphics;

[Autoload(Side = ModSide.Client)]
public class CavernsLightEmitter : ModSystem {
    public static readonly int LightPixelsPadding = 96;

    private static Vector3 _realCavernLight;

    private static bool _appliedHook;

    public override void PreUpdateEntities() {
        Player player = Main.LocalPlayer;

        // Get wanted light for the current biome.
        Vector3 wantedLight = GetWantedLight(player);

        if (wantedLight != Vector3.Zero) {
            // Transition to wanted light color if it's not zero.
            _realCavernLight = Vector3.Lerp(_realCavernLight, wantedLight, 0.1f);
        }
        else if (_realCavernLight != Vector3.Zero) {
            // Otherwise reduce light color. (And set it to zero once it gets really small)
            _realCavernLight *= 0.9f;
            if (_realCavernLight.LengthSquared() < 0.001f) {
                _realCavernLight = Vector3.Zero;
            }
        }

        // Toggle lighting hook if we want cavern light.
        ToggleHook(_realCavernLight != Vector3.Zero && Main.screenPosition.Y + Main.screenHeight >= Main.worldSurface * 16f - LightPixelsPadding);
    }

    private static void ToggleHook(bool apply) {
        if (apply) {
            if (!_appliedHook) {
                On_TileLightScanner.ApplyWallLight += TemporaryLightHook;
                _appliedHook = true;
            }
        }
        else if (_appliedHook) {
            On_TileLightScanner.ApplyWallLight -= TemporaryLightHook;
            _appliedHook = false;
        }
    }

    private static Vector3 GetWantedLight(Player player) {
        if (player.InModBiome<PollutedOceanBiomeUnderground>() || player.InModBiome<PollutedOceanBiomeSurface>()) {
            return PollutedOceanBiomeUnderground.CavernLight * 0.1f;
        }

        return Vector3.Zero;
    }

    private static void TemporaryLightHook(On_TileLightScanner.orig_ApplyWallLight orig, TileLightScanner self, Tile tile, int x, int y, ref Terraria.Utilities.FastRandom localRandom, ref Vector3 lightColor) {
        orig(self, tile, x, y, ref localRandom, ref lightColor);
        if (y > Main.worldSurface && y < Main.UnderworldLayer && BlockAllowsLight(tile, x, y) && WallAllowsLight(tile)) {
            Vector3 cavernLight = _realCavernLight;
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