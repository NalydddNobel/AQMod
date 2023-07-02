using Aequus.Common.EntitySources;
using Aequus.Projectiles.Misc.Bobbers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Misc.Fishing {
    public class NeonGenesis : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accNeonGenesis = Item;
        }
    }
}

namespace Aequus {
    partial class AequusPlayer {
        public Item accNeonGenesis;
        public int timerNeonGenesis;

        private void ResetEffects_NeonGenesis() {
            accNeonGenesis = null;
        }

        private void PostUpdateEquips_NeonGenesis() {
            if (timerNeonGenesis > 0) {
                timerNeonGenesis--;
            }
        }

        public void UseNeonGenesis(Projectile projectile) {
            if (accNeonGenesis == null || timerNeonGenesis > 0) {
                return;
            }

            int target = projectile.FindTargetWithLineOfSight(500f);
            if (target != -1) {
                var source = new EntitySource_ItemUse_WithEntity(projectile, Player, accNeonGenesis);
                Projectile.NewProjectile(
                    source,
                    projectile.Center,
                    Vector2.Normalize(Main.npc[target].Center - projectile.Center) * 25f,
                    ModContent.ProjectileType<NeonFishLaser>(),
                    (int)(Main.player[projectile.owner].HeldItem.fishingPole * (Main.hardMode ? 1f : 1.5f) * accNeonGenesis.EquipmentStacks()),
                    12f,
                    projectile.owner
                );
            }
        }
    }
}