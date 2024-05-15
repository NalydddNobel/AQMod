using Aequus.Core.CodeGeneration;
using Aequus.Old.Content.Materials;
using System;
using Terraria.DataStructures;

namespace Aequus.Old.Content.Items.Accessories.HyperCrystal;

[Gen.AequusPlayer_ResetField<Item>("accHyperCrystal")]
[Gen.AequusPlayer_ResetField<int>("hyperCrystalCooldownMax")]
[Gen.AequusPlayer_ResetField<int>("cHyperCrystal")]
public class HyperCrystal : ModItem {
    public const string TimerKey = "HyperCrystal";

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 1);
        Item.hasVanityEffects = true;
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 255, 255, 200);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequus = player.Aequus();
        aequus.accHyperCrystal = Item;
        if (aequus.hyperCrystalCooldownMax > 0) {
            aequus.hyperCrystalCooldownMax = Math.Max(aequus.hyperCrystalCooldownMax / 2, 1);
        }
        else {
            aequus.hyperCrystalCooldownMax = 60;
        }
    }

    public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        player.Aequus().cHyperCrystal = dyeItem.dye;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<StariteMaterial>(20)
            .AddIngredient(ItemID.FallenStar, 8)
            .AddTile(TileID.Anvils)
            .Register();
    }

    [Gen.AequusItem_UseItem]
    internal static void OnUseItem(Item item, Player player, AequusPlayer aequusPlayer) {
        if (item.damage <= 0 || aequusPlayer.accHyperCrystal == null || aequusPlayer.TimerActive(TimerKey)) {
            return;
        }

        Item hyperCrystal = aequusPlayer.accHyperCrystal;
        IEntitySource source = player.GetSource_Accessory(hyperCrystal);
        aequusPlayer.SetTimer(TimerKey, aequusPlayer.hyperCrystalCooldownMax);
        if (Main.myPlayer == player.whoAmI) {
            Projectile.NewProjectile(source, player.Center, Vector2.Normalize(Main.MouseWorld - player.Center) * 4f,
                ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item) * 2, player.GetWeaponKnockback(item), player.whoAmI, ai0: 3f);
        }
    }
}