using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Conductive;

public class ConductiveProjectile : ModProjectile {
    public override string Texture => AequusTextures.TemporaryBuffIcon;

    public static Point PoweredLocation;
    public static int WireMax = 8;
    public static int ActivationDelay = 60;

    public List<Point> activationPoints;
    private int[,] _wireDataCache;

    public override void SetStaticDefaults() {
    }

    public override void SetDefaults() {
        Projectile.SetDefaultNoInteractions();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = ActivationDelay / 2;
        _wireDataCache = new int[WireMax * 2, WireMax * 2];
        activationPoints = new();
    }

    private void RunConductiveAction(int i, int j, Action<int, int, int, int, Tile> action, int fluff = 0) {
        for (int k = -WireMax + fluff; k < WireMax - fluff; k++) {
            for (int l = -WireMax + fluff; l < WireMax - fluff; l++) {
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
        _wireDataCache[k + WireMax, l + WireMax] = tileCache.Get<TileWallWireStateData>().WireData;
    }

    private void RepairOldWires(int i, int j, int k, int l, Tile tileCache) {
        int bitPack = _wireDataCache[k + WireMax, l + WireMax];
        tileCache.RedWire = TileDataPacking.Unpack(bitPack, 0, 1) > 0;
        tileCache.BlueWire = TileDataPacking.Unpack(bitPack, 1, 1) > 0;
        tileCache.GreenWire = TileDataPacking.Unpack(bitPack, 2, 1) > 0;
        tileCache.YellowWire = TileDataPacking.Unpack(bitPack, 3, 1) > 0;
    }

    private void CreateTemporaryWires(int i, int j, int k, int l, Tile tileCache) {
        if (TileLoader.GetTile(tileCache.TileType) is not ConductiveBlockTile) {
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
            Projectile.ai[0] = 1f;
            PoweredLocation = Projectile.Center.ToTileCoordinates();

            RunConductiveAction(PoweredLocation.X, PoweredLocation.Y, FillWireCache);
            RunConductiveAction(PoweredLocation.X, PoweredLocation.Y, CreateTemporaryWires, fluff: 1);
            try {
                if (Wiring.CheckMech(PoweredLocation.X, PoweredLocation.Y, ActivationDelay)) {
                    Wiring.TripWire(PoweredLocation.X - 1, PoweredLocation.Y - 1, 1, 1);
                }
            }
            catch (Exception ex) {
                Mod.Logger.Error(ex);
            }
            RunConductiveAction(PoweredLocation.X, PoweredLocation.Y, RepairOldWires);

            PoweredLocation = Point.Zero;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        return false;
    }
}