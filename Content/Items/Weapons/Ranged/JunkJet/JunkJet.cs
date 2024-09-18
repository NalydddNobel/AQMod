using Aequus.Common.Utilities;
using Aequus.Content.Items.Materials.CompressedTrash;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Content.Items.Weapons.Ranged.JunkJet;

[LegacyName("Slingshot")]
public class JunkJet : ModItem {
    public static readonly float AmmoReserveChance = 0.5f;

    public static readonly Dictionary<int, JunkJetAmmoInfo> Custom = [];

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ALanguage.Percent(AmmoReserveChance));

    #region Initialization
    public override void SetStaticDefaults() {
#if ELEMENTS
        Element.Air.AddItem(Type);
#endif
        Custom[AmmoID.FallenStar] = new JunkJetAmmoInfo() { ProjectileId = ProjectileID.StarCannonStar, Damage = 50, Knockback = 1f, ProjectileCount = 1, BaseSpread = 0.2f, MaxSpread = 0f, ShootSpeed = 8f };
        Custom[AmmoID.Gel] = new JunkJetAmmoInfo() { ProjectileId = ProjectileID.Flames, Damage = 10, Knockback = -2.3f, ShootSpeed = -1f, AttackSpeedMultiplier = 0.25f, ProjectileCount = 1, BaseSpread = 0.1f, MaxSpread = 0f };
        Custom[ItemID.Ale] = new JunkJetAmmoInfo() { ProjectileId = ProjectileID.Ale, Damage = 14, Knockback = 5f, BaseSpread = 0.1f, MaxSpread = 0.33f };
        Custom[ItemID.SandBlock] = new JunkJetAmmoInfo() { ProjectileId = ProjectileID.SandBallGun, Damage = 30, Knockback = 5f, ProjectileCount = 1, };
    }

    public override void AddRecipes() {
#if POLLUTED_OCEAN
        CreateRecipe()
            .AddIngredient<CompressedTrash>(25)
            .AddTile(TileID.Anvils)
            .Register();
#elif !CRAB_CREVICE_DISABLE
        CreateRecipe()
            .AddIngredient<StarPhish.StarPhish>()
            .AddIngredient<global::Aequus.Items.Materials.PearlShards.PearlShardWhite>(3)
            .AddIngredient<global::Aequus.Items.Materials.Energies.AquaticEnergy>()
            .AddTile<global::Aequus.Tiles.CraftingStations.RecyclingMachineTile>()
            .Register();
#endif
    }

    public override void Unload() {
        Custom.Clear();
    }

    public override void SetDefaults() {
        Item.SetWeaponValues(8, 2f, 0);
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
        Item.useAmmo = AmmoID.Bullet;
        Item.CloneShopValues(ItemID.Trident, +1, 1.5f);
    }
    #endregion

    public override bool? CanChooseAmmo(Item ammo, Player player) {
        if (Custom.ContainsKey(ammo.ammo)) {
            return true;
        }

        if (ItemID.Sets.SandgunAmmoProjectileData[ammo.ammo] != null) {
            return true;
        }

        if (ammo.ammo <= 0) {
            return false;
        }

        return ammo.damage > 0 && !ammo.IsACoin && ammo.DamageType.CountsAsClass(DamageClass.Ranged);
    }

    public override bool CanConsumeAmmo(Item ammo, Player player) {
        return Main.rand.NextFloat() < AmmoReserveChance;
    }

    void GetAmmoData(int bulletItem, int projectileId, out JunkJetAmmoInfo ammoData) {
        // Custom overrides which are specific to the Junk Jet.
        if (Custom.TryGetValue(bulletItem, out var value)) {
            ammoData = value;

            if (value.ProjectileId != ProjectileID.None) {
                return;
            }
        }

        ammoData = new();

        // Rockets.
        if (AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[ItemID.RocketLauncher].TryGetValue(bulletItem, out int rocketConvert)) {
            ammoData.ProjectileId = rocketConvert;
            return;
        }

        // Sand Gun.
        var sandGun = ItemID.Sets.SandgunAmmoProjectileData[bulletItem];
        if (sandGun != null) {
            if (sandGun.ProjectileType != ProjectileID.None) {
                ammoData.ProjectileId = sandGun.ProjectileType;
            }
            else {
                ammoData.ProjectileId = ProjectileID.SandBallGun;
            }

            ammoData.ProjectileCount = 1;

            int sandGunDamage = ContentSamples.ItemsByType[ItemID.Sandgun].damage;
            int junkJetDamage = ContentSamples.ItemsByType[Type].damage;

            ammoData.Damage = sandGunDamage - junkJetDamage + sandGun.BonusDamage;
            return;
        }

        ammoData.ProjectileId = projectileId;
    }

    // Doesn't have entity source, so we cannot get the ammo's item id
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        GetAmmoData(source.AmmoItemIdUsed, type, out var ammoData);
        SetStaticDefaults();
        if (ammoData.AttackSpeedMultiplier != 1f) {
            player.itemTime = (int)(player.itemTimeMax * ammoData.AttackSpeedMultiplier);
        }
        if (ammoData.ShootSpeed != 0f) {
            velocity += Vector2.Normalize(velocity) * ammoData.ShootSpeed;
        }
        for (int i = 0; i < ammoData.ProjectileCount; i++) {
            float spread = ammoData.BaseSpread + ammoData.MaxSpread * (ammoData.ProjectileCount <= 1 ? 1f : i / (float)(ammoData.ProjectileCount - 1));
            var p = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(Main.rand.NextFloat(-spread, spread)), ammoData.ProjectileId, damage + ammoData.Damage, knockback + ammoData.Knockback, player.whoAmI);
            p.timeLeft = Math.Min(p.timeLeft, 600);
            p.noDropItem = true;
        }
        return false;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-2f, 0f);
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage) {
        // Junk Jet already inherits gun damage from its Item.useAmmo parameter.
        damage = damage.CombineWith(player.arrowDamage).CombineWith(player.specialistDamage);
    }
}
