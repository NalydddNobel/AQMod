using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Melee.Yoyo.Pistachiyo;

public class Pistachiyo : ModItem {
    public static int HealAmount = 5;
    public static int MaximumVines = 8;
    public static int MaximumShells = 2;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaximumVines, HealAmount);

    public override void SetStaticDefaults() {
        ItemID.Sets.Yoyo[Item.type] = true;
        ItemID.Sets.GamepadExtraRange[Item.type] = 18;
        ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;

        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useTime = 25;
        Item.useAnimation = 25;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.UseSound = SoundID.Item1;

        Item.damage = 58;
        Item.DamageType = DamageClass.MeleeNoSpeed;
        Item.knockBack = 2.5f;
        Item.channel = true;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.buyPrice(gold: 4);

        Item.shoot = ModContent.ProjectileType<PistachiyoProj>();
        Item.shootSpeed = 16f;
    }

    public override void HoldItem(Player player) {
        if (player.stringColor == 0) {
            player.stringColor = 4; // Makes the string green
        }
    }

    public override void AddRecipes() {
        /*
         * CreateRecipe()
         *    .AddIngredient<CrackinClam>()
         *    .AddIngredient<OrganicEnergy>()
         *    .AddTile(TileID.MythrilAnvil)
         *    .Register();
         */
    }
}