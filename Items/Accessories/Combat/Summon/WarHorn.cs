using Aequus.Common.Net.Sounds;
using Aequus.Items.Accessories.Combat.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.Summon {
    public class WarHorn : ModItem {
        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accWarHorn++;
        }
    }

    public class WarHornCooldown : ModBuff {
        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) {
            player.Aequus().cooldownWarHorn = true;
        }
    }
}

namespace Aequus.Projectiles {
    partial class AequusProjectile {
        private void Proc_Warhorn(Projectile projectile, NPC target, NPC.HitInfo hit) {
            int proj = (projectile.minion || projectile.sentry) ? projectile.whoAmI : Helper.FindProjectileIdentity(projectile.owner, sourceProjIdentity);
            if (proj != -1) {
                var aequus = Main.projectile[proj].Aequus();
                Main.player[Main.projectile[proj].owner].AddBuff(ModContent.BuffType<WarHornCooldown>(), 600);
                if (aequus.warHornFrenzy == 0) {
                    aequus.extraUpdatesTemporary++;
                }
                if (aequus.warHornFrenzy <= 30) {
                    ModContent.GetInstance<WarHornSound>().Play(projectile.Center);
                }
                aequus.warHornFrenzy = (ushort)(240 * Main.player[projectile.owner].Aequus().accWarHorn);
                for (int i = 0; i < 20; i++) {
                    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height,
                        DustID.RedTorch, Scale: Main.rand.NextFloat(0.8f, 1.5f));
                    d.noGravity = true;
                    d.velocity *= 5f;
                    d.fadeIn = d.scale + 0.2f;
                    d.noLightEmittence = true;
                }
                Main.projectile[proj].netUpdate = true;
            }
        }
        private void OnHit_Warhorn(Projectile projectile, NPC target, NPC.HitInfo hit) {
            var aequusPlayer = Main.player[projectile.owner].Aequus();
            if (aequusPlayer.accWarHorn <= 0 || aequusPlayer.cooldownWarHorn) {
                return;
            }
            Proc_Warhorn(projectile, target, hit);
        }
    }
}

namespace Aequus {
    partial class AequusPlayer {
        public int accWarHorn;
        public bool cooldownWarHorn;

        private void ResetEffects_WarHorn() {
            accWarHorn = 0;
            cooldownWarHorn = false;
        }
    }
}