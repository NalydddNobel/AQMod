using Aequus.Common.Items;
using Aequus.Content.DataSets;
using Aequus.Content.Materials;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Content.Weapons.Ranged.Misc.JunkJet;

[LegacyName("Slingshot")]
public class JunkJet : ModItem {
    public struct AmmoData {
        public Int32 ProjectileId;
        public Int32 ProjectileCount;
        public Int32 Damage;
        public Single Knockback;
        public Single ShootSpeed;
        public Single AttackSpeedMultiplier;
        public Single BaseSpread;
        public Single MaxSpread;

        public AmmoData() {
            ProjectileId = 0;
            Damage = 0;
            Knockback = 0f;
            ShootSpeed = 4f;
            AttackSpeedMultiplier = 1f;
            ProjectileCount = 3;
            BaseSpread = 0f;
            MaxSpread = 0.15f;
        }
    }

    public static Single AmmoReserveChance { get; set; } = 0.5f;

    public static readonly Dictionary<Int32, AmmoData> AmmoOverrides = new();

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Percent(AmmoReserveChance));

    #region Initialization
    public override void SetStaticDefaults() {
        AmmoOverrides[AmmoID.FallenStar] = new AmmoData() { ProjectileId = ProjectileID.StarCannonStar, Damage = 50, Knockback = 1f, ProjectileCount = 1, BaseSpread = 0.2f, MaxSpread = 0f, ShootSpeed = 8f };
        AmmoOverrides[AmmoID.Gel] = new AmmoData() { ProjectileId = ProjectileID.Flames, Damage = 10, Knockback = -2.3f, ShootSpeed = -1f, AttackSpeedMultiplier = 0.25f, ProjectileCount = 1, BaseSpread = 0.1f, MaxSpread = 0f };
        AmmoOverrides[ItemID.SandBlock] = new AmmoData() { ProjectileId = ProjectileID.SandBallGun, Damage = 20, Knockback = 3f, ProjectileCount = 1, BaseSpread = 0f, MaxSpread = 0f };
        AmmoOverrides[ItemID.EbonsandBlock] = new AmmoData() { ProjectileId = ProjectileID.EbonsandBallGun, Damage = 20, Knockback = 3f, ProjectileCount = 1, BaseSpread = 0f, MaxSpread = 0f };
        AmmoOverrides[ItemID.CrimsandBlock] = new AmmoData() { ProjectileId = ProjectileID.CrimsandBallGun, Damage = 20, Knockback = 3f, ProjectileCount = 1, BaseSpread = 0f, MaxSpread = 0f };
        AmmoOverrides[ItemID.PearlsandBlock] = new AmmoData() { ProjectileId = ProjectileID.PearlSandBallGun, Damage = 20, Knockback = 3f, ProjectileCount = 1, BaseSpread = 0f, MaxSpread = 0f };
        AmmoOverrides[ItemID.Ale] = new AmmoData() { ProjectileId = ProjectileID.Ale, Damage = 14, Knockback = 5f, BaseSpread = 0.1f, MaxSpread = 0.33f };
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<CompressedTrash>(12)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void Unload() {
        AmmoOverrides.Clear();
    }

    public override void SetDefaults() {
        Item.SetWeaponValues(3, 2f, 4);
        Item.DamageType = DamageClass.Ranged;
        Item.useTime = 32;
        Item.useAnimation = 32;
        Item.width = 24;
        Item.height = 24;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 6f;
        Item.UseSound = AequusSounds.JunkJetShoot;
        Item.noMelee = true;
        Item.autoReuse = true;
        Item.rare = ItemCommons.Rarity.CrabsonLoot;
        Item.value = ItemCommons.Price.CrabsonLoot;
        Item.useAmmo = AmmoID.Bullet;
    }
    #endregion

    public override Boolean? CanChooseAmmo(Item ammo, Player player) {
        return ammo.ammo > 0 && (AmmoOverrides.ContainsKey(ammo.ammo) || ammo.damage > 0 && !ammo.IsACoin && ammo.DamageType.CountsAsClass(DamageClass.Ranged));
    }

    public override Boolean CanConsumeAmmo(Item ammo, Player player) {
        return Main.rand.NextFloat() < AmmoReserveChance;
    }

    public static void GetAmmoData(Int32 bulletItem, Int32 projectileId, out AmmoData ammoData) {
        if (AmmoOverrides.TryGetValue(bulletItem, out var value)) {
            ammoData = value;
            if (value.ProjectileId == ProjectileID.None) {
                ammoData.ProjectileId = bulletItem;
            }
            return;
        }

        ammoData = new();
        if (ItemSets.AmmoIdToProjectileId.TryGetValue(bulletItem, out var projectileConversion)) {
            ammoData.ProjectileId = projectileConversion;
            return;
        }
        ammoData.ProjectileId = projectileId;
    }

    // Doesn't have entity source, so we cannot get the ammo's item id
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref Int32 type, ref Int32 damage, ref Single knockback) {
    }

    public override Boolean Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, Int32 type, Int32 damage, Single knockback) {
        GetAmmoData(source.AmmoItemIdUsed, type, out var ammoData);
        SetStaticDefaults();
        if (ammoData.AttackSpeedMultiplier != 1f) {
            player.itemTime = (Int32)(player.itemTimeMax * ammoData.AttackSpeedMultiplier);
        }
        if (ammoData.ShootSpeed != 0f) {
            velocity += Vector2.Normalize(velocity) * ammoData.ShootSpeed;
        }
        for (Int32 i = 0; i < ammoData.ProjectileCount; i++) {
            Single spread = ammoData.BaseSpread + ammoData.MaxSpread * (ammoData.ProjectileCount <= 1 ? 1f : i / (Single)(ammoData.ProjectileCount - 1));
            var p = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(Main.rand.NextFloat(-spread, spread)), ammoData.ProjectileId, damage + ammoData.Damage, knockback + ammoData.Knockback, player.whoAmI);
            p.timeLeft = Math.Min(p.timeLeft, 600);
            p.noDropItem = true;
        }
        return false;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-2f, 0f);
    }
}