using Aequu2.Core.CodeGeneration;
using Aequu2.Old.Content.Items.Materials;
using System;
using Terraria.DataStructures;

namespace Aequu2.Old.Content.Items.Accessories.HyperCrystal;

[Gen.Aequu2Player_ResetField<Item>("accHyperCrystal")]
[Gen.Aequu2Player_ResetField<int>("hyperCrystalCooldownMax")]
[Gen.Aequu2Player_ResetField<int>("cHyperCrystal")]
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
        var Aequu2 = player.Aequu2();
        Aequu2.accHyperCrystal = Item;
        if (Aequu2.hyperCrystalCooldownMax > 0) {
            Aequu2.hyperCrystalCooldownMax = Math.Max(Aequu2.hyperCrystalCooldownMax / 2, 1);
        }
        else {
            Aequu2.hyperCrystalCooldownMax = 60;
        }
    }

    public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        player.Aequu2().cHyperCrystal = dyeItem.dye;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<StariteMaterial>(20)
            .AddIngredient(ItemID.FallenStar, 8)
            .AddTile(TileID.Anvils)
            .Register();
    }

    [Gen.Aequu2Item_UseItem]
    internal static void OnUseItem(Item item, Player player, Aequu2Player Aequu2Player) {
        if (item.damage <= 0 || Aequu2Player.accHyperCrystal == null || Aequu2Player.TimerActive(TimerKey)) {
            return;
        }

        Item hyperCrystal = Aequu2Player.accHyperCrystal;
        IEntitySource source = player.GetSource_Accessory(hyperCrystal);
        Aequu2Player.SetTimer(TimerKey, Aequu2Player.hyperCrystalCooldownMax);
        if (Main.myPlayer == player.whoAmI) {
            Projectile.NewProjectile(source, player.Center, Vector2.Normalize(Main.MouseWorld - player.Center) * 4f,
                ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item) * 2, player.GetWeaponKnockback(item), player.whoAmI, ai0: 3f);
        }
    }
}