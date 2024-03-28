using Aequus.Content.Biomes.PollutedOcean.Tiles.Scrap;
using Aequus.Core.CodeGeneration;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Utilities;

namespace Aequus;

public partial class AequusPlayer {
    public bool oldTouchingScrapBlock;
    [ResetEffects]
    public bool touchingScrapBlock;

    public static int GetScrapDustId(UnifiedRandom random) {
        return random.Next(3) switch {
            2 => DustID.Lead,
            1 => DustID.Tin,
            _ => DustID.Copper,
        };
    }
    public static int GetScrapGoreId(UnifiedRandom random) {
        return random.Next(698, 704);
    }

    public void UpdateScrapBlockState() {
        if (!touchingScrapBlock) {
            oldTouchingScrapBlock = false;
            return;
        }

        if (!oldTouchingScrapBlock) {
            if (Player.velocity.Length() > 4f) {
                SoundEngine.PlaySound(SoundID.Splash);
            }
            else {
                SoundEngine.PlaySound(SoundID.SplashWeak);
            }
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

        bool pressedJump = Player.controlJump && Player.releaseJump;
        for (int i = 0; i < playerTileHeight; i++) {
            if (Main.tile[x, bottomY - i].HasUnactuatedTile && Main.tile[x, bottomY - i].TileType == ModContent.TileType<ScrapBlock>()) {
                amountCovered++;

                if (Main.netMode != NetmodeID.Server) {
                    if (pressedJump || Math.Abs(Player.velocity.X) > 0.3f || Math.Abs(Player.velocity.Y) > 0.5f || Main.rand.NextBool(30)) {
                        if (pressedJump || Player.miscCounter % 12 == 0) {
                            SoundEngine.PlaySound(SoundID.SplashWeak);
                            Dust.NewDust(new Vector2(x, bottomY - i).ToWorldCoordinates(0f, 0f), 16, 16, GetScrapDustId(Main.rand));
                            if (Main.rand.NextBool(5)) {
                                Gore.NewGore(new EntitySource_TileInteraction(Player, x, bottomY - i, "Aequus: Scrap Block"), new Vector2(x, bottomY - i).ToWorldCoordinates() + Main.rand.NextVector2Square(-8f, 8f), Main.rand.NextVector2Unit(), GetScrapGoreId(Main.rand));
                            }
                        }
                    }
                }
            }
        }

        if (amountCovered >= Math.Min(playerTileHeight, 2)) {
            if (Player.velocity.Y * Player.gravDir < 0f) {
                Player.velocity.Y *= 0.7f;
            }
            if (!Player.controlDown && Player.velocity.Y * Player.gravDir > 0f) {
                Player.velocity.Y = 0f;
                Player.gravity = -Player.gravity * 0.3f;
            }
        }

        if (amountCovered >= 1) {
            Player.velocity.X *= 0.9f;
            Player.wingTime = Player.wingTimeMax;
            Player.rocketTime = Player.rocketTimeMax;
            Player.RefreshExtraJumps();

            float jumpSpeed = (Player.jumpSpeed + Player.jumpSpeedBoost) / 2f;
            if (Player.accFlipper) {
                jumpSpeed += 2f;
            }

            if (pressedJump) {
                Player.velocity.Y = -jumpSpeed * Player.gravDir;
            }

            if (Player.velocity.Y * Player.gravDir > 0.5f) {
                if (!oldTouchingScrapBlock) {
                    int amount = (int)(Player.velocity.Y * 3f);
                    var effectPosition = Player.Bottom;
                    for (int i = 0; i < amount; i++) {
                        Dust.NewDustPerfect(effectPosition + Main.rand.NextVector2Square(-8f, 8f), GetScrapDustId(Main.rand), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-amount / 10f, 0f)), Scale: Main.rand.NextFloat(0.75f, 2f));
                        if (Main.rand.NextBool(5)) {
                            Gore.NewGore(new EntitySource_TileInteraction(Player, x, bottomY, "Aequus: Scrap Block"), effectPosition + Main.rand.NextVector2Square(-8f, 8f), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-amount / 10f, 0f)), GetScrapGoreId(Main.rand));
                        }
                    }
                }
                Player.velocity.Y = 2f * Player.gravDir;
            }
        }

        oldTouchingScrapBlock = true;
    }
}