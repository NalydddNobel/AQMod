using Aequus.Common.Items.SentryChip;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.SentryChip {
    public class WaterWalkingBootsInteraction : SentryInteraction {
        public override void Load(Mod mod) {
            AddTo(ItemID.WaterWalkingBoots);
            AddTo(ItemID.ObsidianWaterWalkingBoots);
            AddTo(ItemID.LavaWaders);
            AddTo(ItemID.TerrasparkBoots);
        }

        public override void OnSentryAI(SentryAccessoryInfo info) {
            var tileCoords = new Vector2(info.Projectile.Center.X, info.Projectile.position.Y + info.Projectile.height - 8f).ToTileCoordinates();
            if (info.Projectile.wet || !WorldGen.InWorld(tileCoords.X, tileCoords.Y, 10) || Main.tile[tileCoords].LiquidAmount > 128) {
                return;
            }

            for (int l = 0; l < 2; l++) {
                if (Main.tile[tileCoords].LiquidAmount == 255) {
                    info.Projectile.velocity.Y = Math.Min(info.Projectile.velocity.Y, 0f);
                    for (int j = 0; j < 10; j++) {
                        info.Projectile.Bottom = new Vector2(info.Projectile.Bottom.X, (tileCoords.Y - j) * 16f + Main.tile[tileCoords].LiquidAmount / 255f * 16f);
                        if (Main.tile[tileCoords.X, tileCoords.Y - j].LiquidAmount < 255) {
                            break;
                        }
                    }
                }
                tileCoords.Y++;
            }
        }
    }
}