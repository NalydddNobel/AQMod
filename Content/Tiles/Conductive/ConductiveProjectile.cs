using Aequus.Common.Wires;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace Aequus.Content.Tiles.Conductive;

public class ConductiveProjectile : ModProjectile {
    public override string Texture => AequusTextures.TemporaryBuffIcon;

    public List<Point> activationPoints;
    private int[,] _wireDataCache;

    public override void SetStaticDefaults() {
    }

    public override void SetDefaults() {
        Projectile.SetDefaultNoInteractions();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = ConductiveSystem.ActivationDelay / 2;
        _wireDataCache = new int[ConductiveSystem.WireMax * 2, ConductiveSystem.WireMax * 2];
        activationPoints = new();
    }

    private void RunConductiveAction(int i, int j, Action<int, int, int, int, Tile> action, int fluff = 0) {
        for (int k = -ConductiveSystem.WireMax + fluff; k < ConductiveSystem.WireMax - fluff; k++) {
            for (int l = -ConductiveSystem.WireMax + fluff; l < ConductiveSystem.WireMax - fluff; l++) {
                int x = i + k;
                int y = j + l;
                if (!WorldGen.InWorld(x, y, 10)) {
                    continue;
                }

                var tile = Framing.GetTileSafely(x, y);
                action(i, j, k, l, tile);
            }
        }
    }

    private void FillWireCache(int i, int j, int k, int l, Tile tileCache) {
        _wireDataCache[k + ConductiveSystem.WireMax, l + ConductiveSystem.WireMax] = tileCache.Get<TileWallWireStateData>().WireData;
    }

    private void RepairOldWires(int i, int j, int k, int l, Tile tileCache) {
        int bitPack = _wireDataCache[k + ConductiveSystem.WireMax, l + ConductiveSystem.WireMax];
        tileCache.RedWire = TileDataPacking.Unpack(bitPack, 0, 1) > 0;
        tileCache.BlueWire = TileDataPacking.Unpack(bitPack, 1, 1) > 0;
        tileCache.GreenWire = TileDataPacking.Unpack(bitPack, 2, 1) > 0;
        tileCache.YellowWire = TileDataPacking.Unpack(bitPack, 3, 1) > 0;
    }

    private void CreateTemporaryWires(int i, int j, int k, int l, Tile tileCache) {
        if (tileCache.TileType != Main.tile[i, j].TileType) {
            return;
        }
        for (int m = -1; m < 2; m++) {
            for (int n = -1; n < 2; n++) {
                var tile = Framing.GetTileSafely(i + k + m, j + l + n);
                tile.RedWire = true;
                tile.BlueWire = false;
                tile.GreenWire = false;
                tile.YellowWire = false;
            }
        }
    }

    public override void AI() {
        if (Projectile.ai[0] == 0f) {
            if (Main.netMode != NetmodeID.Server) {
                float shortenedVolume = 1f - Projectile.Distance(Main.LocalPlayer.Center) / 240f;
                if (shortenedVolume > 0f) {
                    SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap with { Pitch = -0.2f, Volume = shortenedVolume }, Projectile.Center);
                }
            }

            Projectile.ai[0] = 1f;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                return;
            }

            float cooldownMultiplier = WiringSystem.MechCooldownMultiplier;
            WiringSystem.MechCooldownMultiplier = 0.5f;
            ConductiveSystem.PoweredLocation = Projectile.Center.ToTileCoordinates();

            RunConductiveAction(ConductiveSystem.PoweredLocation.X, ConductiveSystem.PoweredLocation.Y, FillWireCache);
            RunConductiveAction(ConductiveSystem.PoweredLocation.X, ConductiveSystem.PoweredLocation.Y, CreateTemporaryWires, fluff: 1);
            try {
                Wiring.TripWire(ConductiveSystem.PoweredLocation.X, ConductiveSystem.PoweredLocation.Y, 1, 1);
            }
            catch (Exception ex) {
                Mod.Logger.Error(ex);
            }
            RunConductiveAction(ConductiveSystem.PoweredLocation.X, ConductiveSystem.PoweredLocation.Y, RepairOldWires);

            WiringSystem.MechCooldownMultiplier = cooldownMultiplier;
            ConductiveSystem.PoweredLocation = Point.Zero;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        return false;
    }
}