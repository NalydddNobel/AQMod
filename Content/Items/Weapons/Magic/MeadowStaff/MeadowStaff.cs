﻿using Aequus.Common;
using Aequus.Common.Entities.Items;
using System;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Content.Items.Weapons.Magic.MeadowStaff;

[WorkInProgress]
public class MeadowStaff : ModItem, IHaveCrossedOutIndicator {
    public override void SetStaticDefaults() {
        Item.staff[Type] = true;
    }

    public override void SetDefaults() {
        Item.mana = 0;
        Item.UseSound = SoundID.Item43;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.damage = 18;
        Item.useAnimation = 34;
        Item.useTime = 34;
        Item.width = 40;
        Item.height = 40;
        Item.shoot = ModContent.ProjectileType<MeadowStaffProj>();
        Item.shootSpeed = 7.5f;
        Item.knockBack = 4f;
        Item.value = Item.sellPrice(copper: 20);
        Item.DamageType = DamageClass.Magic;
        Item.autoReuse = true;
        Item.noMelee = true;
    }

    public override bool CanUseItem(Player player) {
        return !player.manaSick;
    }

    public override bool AltFunctionUse(Player player) {
        return player.statMana > 1;
    }

    public override bool? UseItem(Player player) {

        return base.UseItem(player);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        int mana = player.statMana;
        if (player.altFunctionUse == 2 && player.CheckMana(mana, pay: true)) {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<MeadowStaffLaser>(), damage, knockback, player.whoAmI, ai0: mana);
            return false;
        }
        return true;
    }

    public override void AddRecipes() {
#if !DEBUG
        return;
#endif
        CreateRecipe()
            .AddIngredient(Instance<Biomes.Meadows.Tiles.MeadowWood>().Item.Type, 14)
            .AddTile(TileID.WorkBenches)
            .Register();
    }

    bool IHaveCrossedOutIndicator.Active() {
        return !CanUseItem(Main.LocalPlayer);
    }
}

public class MeadowStaffProj : ModProjectile {
    public override LocalizedText DisplayName => Instance<MeadowStaff>().DisplayName;
    public override void SetDefaults() {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
    }
    public override void AI() {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor) {
        return base.PreDraw(ref lightColor);
    }
}

public class MeadowStaffLaser : ModProjectile {
    const int DistanceFromPlayer = 60;
    const int MaxDistance = 800;
    const int StepDistance = 6;

    public override LocalizedText DisplayName => Instance<MeadowStaff>().DisplayName;

    public float Length { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }

    public override void SetDefaults() {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.hide = true;
    }

    public override void AI() {
        Player player = Main.player[Projectile.owner];
        player.statMana = 0;
        player.manaRegen = 0;
        player.manaRegenCount = 0;

        Projectile.damage = (int)Math.Max(Projectile.ai[0] * 0.5f, 10f);
        Projectile.friendly = true;

        // Keep projectile on the player's center.
        Projectile.Center = player.Center;

        // Ensure velocity is equal to 1.
        Projectile.velocity.Normalize();

        ScanLength(player);

        if (Projectile.localAI[0] == 0f) {
            Projectile.localAI[0]++;
            Helper.Punch(Projectile.velocity, 6f, 4f, 60);

            for (int i = DistanceFromPlayer; i < Length; i += 4) {
                Vector2 dustPos = Projectile.position + Projectile.velocity * i;
                Dust d = Dust.NewDustDirect(dustPos, Projectile.width, Projectile.height, DustID.GemSapphire, Scale: Main.rand.NextFloat(1f, 2f));
                d.velocity *= 0.6f;
                d.fadeIn = 1.5f;
                d.noGravity = true;
            }
            for (int i = 0; i < 40; i++) {
                Vector2 dustPos = Projectile.position + Projectile.velocity * Length;
                Dust d = Dust.NewDustDirect(dustPos, Projectile.width, Projectile.height, DustID.GemSapphire, Scale: Main.rand.NextFloat(1f, 2f));
                d.velocity *= 2f;
                d.velocity -= Projectile.velocity * Main.rand.NextFloat(6f);
                d.fadeIn = 1.5f;
                d.noGravity = true;
            }
        }
        else {
            float velocity = 0f;
            Vector2 dustOffset = Vector2.Zero;
            for (int i = DistanceFromPlayer; i < Length; i += 2) {
                if (Main.rand.NextBool(4)) {
                    continue;
                }

                if (Main.rand.NextBool(40)) {
                    velocity = -velocity;
                    velocity += Main.rand.NextFloat(-0.3f, 0.3f);
                }

                dustOffset += Projectile.velocity.RotatedBy(MathHelper.PiOver2) * velocity;

                Vector2 realOffset = dustOffset;
                if (i > Length / 2f) {
                    realOffset *= 1f - (i - Length / 2f) / (Length / 2f);
                }

                Vector2 dustPos = Projectile.Center + Projectile.velocity * i + realOffset;
                Dust d = Dust.NewDustPerfect(dustPos, DustID.GemSapphire, Scale: 0.8f);
                d.alpha = Projectile.alpha;
                if (Main.rand.NextBool(4)) {
                    d.velocity *= 0.6f;
                    d.noGravity = true;
                }
                else {
                    d.velocity *= 0.1f;
                    d.noGravity = true;
                }
            }
        }
        Projectile.alpha += 20;
        if (Projectile.alpha >= 255) {
            Projectile.Kill();
        }
    }

    void ScanLength(Player player) {
        for (Length = DistanceFromPlayer; Length <= MaxDistance; Length += StepDistance) {
            Vector2 start = player.Center + Projectile.velocity * Length;
            if (!Collision.CanHit(player.Center, 1, 1, start, 1, 1)) {
                Length -= StepDistance;
                break;
            }
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        Player player = Main.player[Projectile.owner];
        Vector2 unit = Projectile.velocity;
        float point = 0f;
        // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
        // It will look for collisions on the given line using AABB
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
            player.Center + unit * Length, projHitbox.Width * 2, ref point);
    }

    public override bool PreDraw(ref Color lightColor) {
        return base.PreDraw(ref lightColor);
    }
}