using Aequus;
using Aequus.Common.Items;
using Aequus.Content.DataSets;
using Aequus.Content.Items.Material;
using Aequus.Content.Items.Material.Energy.Aquatic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Ranged.Misc.JunkJet;

public class JunkJet : ModItem {
    public static float AmmoReserveChance = 0.5f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Percent(AmmoReserveChance));

    public override void SetDefaults() {
        Item.SetWeaponValues(3, 2f, 4);
        Item.DamageType = DamageClass.Ranged;
        Item.useTime = 32;
        Item.useAnimation = 32;
        Item.width = 30;
        Item.height = 30;
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

    public override bool? CanChooseAmmo(Item ammo, Player player) {
        return ammo.ammo > 0 && ammo.damage > 0 && !ammo.IsACoin && ammo.DamageType.CountsAsClass(DamageClass.Ranged);
    }

    public override bool CanConsumeAmmo(Item ammo, Player player) {
        return Main.rand.NextFloat() < AmmoReserveChance;
    }

    public static void GetConversionType(int bulletItem, ref int projectileId) {
        if (ItemSets.AmmoIdToProjectileId.TryGetIdValue(bulletItem, out int projectileConversion)) {
            projectileId = projectileConversion;
        }
    }

    // Doesn't have entity source, so we cannot get the ammo's item id
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        GetConversionType(source.AmmoItemIdUsed, ref type);
        for (int i = 0; i < 3; i++) {
            var p = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.075f, 0.075f) * i), type, damage, knockback, player.whoAmI);
            p.timeLeft = Math.Min(p.timeLeft, 600);
        }
        return false;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-2f, 0f);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<CompressedTrash>(3)
            .AddIngredient<AquaticEnergy>()
            .AddTile(TileID.Extractinator)
            .Register();
    }
}