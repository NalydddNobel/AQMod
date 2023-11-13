using Aequus.Content.Biomes.PollutedOcean.Tiles.Scrap;
using Aequus.Core.Generator;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool touchingScrapBlock;

    public void UpdateScrapBlockState() {
        if (!touchingScrapBlock) {
            return;
        }

        int amountCovered = 0;
        var centerTileCoords = Player.Center.ToTileCoordinates();
        int x = centerTileCoords.X;
        int topY = Player.Top.ToTileCoordinates().Y;
        int bottomY = Player.Bottom.ToTileCoordinates().Y;
        int playerTileHeight = Math.Max(bottomY - topY, 1);

        if (!WorldGen.InWorld(x, topY) || !WorldGen.InWorld(x, bottomY)) {
            return;
        }

        for (int i = 0; i < playerTileHeight; i++) {
            if (Main.tile[x, bottomY - i].HasUnactuatedTile && Main.tile[x, bottomY - i].TileType == ModContent.TileType<ScrapBlock>()) {
                amountCovered++;
            }
        }

        if (amountCovered >= Math.Min(playerTileHeight, 2)) {
            Player.velocity.X *= 0.9f;
            if (Player.velocity.Y * Player.gravDir < 0f) {
                Player.velocity.Y *= 0.7f;
            }
        }

        if (amountCovered >= 1) {
            Player.wingTime = Player.wingTimeMax;
            Player.rocketTime = Player.rocketTimeMax;
            Player.RefreshExtraJumps();

            float jumpSpeed = 4f + Player.jumpSpeedBoost;
            if (Player.accFlipper) {
                jumpSpeed += 2f;
            }

            if (Player.controlJump && Player.releaseJump) {
                Player.velocity.Y = -jumpSpeed * Player.gravDir;
            }

            if (Player.velocity.Y * Player.gravDir > 0.5f) {
                Player.velocity.Y = 0.5f * Player.gravDir;
            }
        }
    }
}