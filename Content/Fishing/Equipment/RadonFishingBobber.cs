using Aequus.Items.Accessories.CrownOfBlood;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.Equipment {
    public class RadonFishingBobber : ModItem {
        public override void SetStaticDefaults() {
            CrownOfBloodItem.NoBoost.Add(Type);
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.FishingBobberGlowingRainbow);
        }

        private void UpdateFishingBobber(Player player) {
            player.overrideFishingBobber = ModContent.ProjectileType<RadonFishingBobberProj>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.accFishingBobber = true;
            if (!hideVisual) {
                UpdateFishingBobber(player);
            }
            if (player.HeldItem.IsAir || player.HeldItem.fishingPole <= 0) {
                return;
            }
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (Main.projectile[i].active && Main.projectile[i].bobber && Main.projectile[i].owner == player.whoAmI) {
                    player.AddBuff(ModContent.BuffType<RadonFishingBobberBuff>(), 8);
                }
            }
        }

        public override void UpdateVanity(Player player) {
            UpdateFishingBobber(player);
        }
    }

    public class RadonFishingBobberBuff : ModBuff {
        public override void SetStaticDefaults() {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) {
            player.calmed = true;
            player.aggro -= 400;
        }
    }

    public class RadonFishingBobberProj : ModProjectile {
        public override void SetDefaults() {
            Projectile.CloneDefaults(ProjectileID.FishingBobberGlowingRainbow);
            Projectile.light = 0f;
            Projectile.glowMask = -1;
            AIType = ProjectileID.FishingBobber;
        }

        public override void ModifyFishingLine(ref Vector2 lineOriginOffset, ref Color lineColor) {
            if (Main.player[Projectile.owner].HeldItem.type >= ItemID.Count) {
                return;
            }

            if (Main.player[Projectile.owner].direction == -1) {
                lineOriginOffset.X -= 13f; // Stupid
            }
        }
    }
}