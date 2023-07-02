using Aequus.Buffs.Debuffs;
using Aequus.Common.Buffs;
using Aequus.Common.Items;
using Aequus.Common.Net.Sounds;
using Aequus.Content.Events.DemonSiege;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Demon.HellsBoon {
    [AutoloadGlowMask]
    public class HellsBoon : ModItem {
        private int slashTimer;

        public override void SetStaticDefaults() {
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ItemID.LightsBane, Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.damage = 20;
            Item.useTime = 52;
            Item.useAnimation = 26;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = ItemDefaults.ValueDemonSiege;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 3f;
            Item.shootSpeed = 35f;
            Item.shoot = ModContent.ProjectileType<HellsBoonSpawner>();
            Item.scale = 1f;
        }

        public override Color? GetAlpha(Color lightColor) {
            return lightColor.MaxRGBA(50);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuffs(240, 1, CorruptionHellfire.Debuffs);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) {
            AequusBuff.ApplyBuff<CorruptionHellfire>(player, 240, out bool canPlaySound);
            if (canPlaySound) {
                ModContent.GetInstance<BlueFireDebuffSound>().Play(target.Center, pitchOverride: -0.2f);
            }
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame) {
            if (slashTimer > 0) {
                slashTimer--;
                return;
            }

            slashTimer = player.itemTimeMax / 3;
            if (Main.myPlayer != player.whoAmI) {
                return;
            }

            int target = Helper.FindTargetWithLineOfSight(player.Center, maxRange: 196f);
            Vector2 spawnLocation;
            Vector2 velocity;
            if (target != -1) {
                spawnLocation = Main.npc[target].Center + Main.rand.NextVector2Square(-20f, 20f);
                velocity = player.DirectionTo(Main.npc[target]);
            }
            else {
                spawnLocation = player.Center + new Vector2(50f * player.direction, (12f + player.height / 2f) * -player.gravDir) * Item.scale;
                velocity = new Vector2(player.direction * Main.rand.NextFloat(1f), player.gravDir);
            }

            int damage = player.GetWeaponDamage(Item) / 2;
            float ai0 = 1f;
            if (player.RollCrit(Item)) {
                damage *= 2;
                ai0 = 2f;
            }

            Projectile.NewProjectile(player.GetSource_ItemUse(Item), spawnLocation, Vector2.Normalize(velocity).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 0.001f, ProjectileID.LightsBane, damage, player.GetWeaponKnockback(Item), player.whoAmI, ai0);
        }
        public override bool? UseItem(Player player) {
            
            return base.UseItem(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
            damage = (int)(damage * 0.5f);
            position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
        }
    }
}