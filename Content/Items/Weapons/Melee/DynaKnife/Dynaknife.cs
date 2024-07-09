using AequusRemake.Core.Util.Helpers;
using AequusRemake.Systems.Items;
using AequusRemake.Systems.Particles.Common;
using AequusRemake.Systems.Synergy;
using System;
using tModLoaderExtended.GlowMasks;

namespace AequusRemake.Content.Items.Weapons.Melee.DynaKnife;

[LegacyName("CrystalDagger", "Mallet")]
[AutoloadGlowMask]
[Replacement]
public class Dynaknife : ModItem, ICooldownItem {
    public int CooldownTime => 120;

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.SetWeaponValues(20, 0.1f, 46);
        Item.DamageType = DamageClass.Melee;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 10);
        Item.shootSpeed = 2f;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<DynaknifeProj>();
        Item.scale = 1.1f;
    }

    public override bool AltFunctionUse(Player player) {
        return !this.HasCooldown(player);
    }

    public void RClickUse(Player player) {
        this.SetCooldown(player);

        int dir = player.direction;
        if (Main.myPlayer == player.whoAmI) {
            dir = Math.Sign(Main.MouseWorld.X - player.Center.X);
        }
        player.velocity.X = 12f * dir;
        player.RemoveAllGrapplingHooks();

        if (Main.netMode != NetmodeID.Server) {
            Rectangle hitbox = player.getRect();
            if (Cull.ClipXYWH(hitbox)) {
                foreach (var particle in DashParticles.NewMultipleReduced(16, 8)) {
                    particle.Location = Main.rand.NextVector2FromRectangle(hitbox);
                    particle.Velocity = new Vector2(dir * Main.rand.NextFloat(10f, 16f), 0f);
                    particle.Rotation = particle.Velocity.ToRotation() + MathHelper.Pi;
                    particle.Scale = Main.rand.NextFloat(1f, 1.5f);
                }
            }
        }
    }

    public override bool? UseItem(Player player) {
        if (player.altFunctionUse != 2) {
            return null;
        }

        RClickUse(player);
        return true;
    }
}
