using Aequus.Content.Items.Material;

namespace Aequus.Content.Items.Tools.Glowsticks.SuperGlowstick;

public class SuperGlowstick : ModItem {
    public static int GlowstickRecipeCount { get; set; } = 25;

    public static float JumpSpeedBoost { get; set; } = 0.66f;

    public static float MaxFallSpeedMultiplier { get; set; } = 0.95f;
    public static float FallGravityMultiplier { get; set; } = 0.95f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 25);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.jumpSpeedBoost += JumpSpeedBoost;
        player.spikedBoots++;

        if (Main.netMode != NetmodeID.Server) {
            Lighting.AddLight(player.Center, new Vector3(1f, 1f, 1f));
            Main.instance.SpelunkerProjectileHelper.AddSpotToCheck(player.Center);
        }

        if (!player.controlDown) {
            player.maxFallSpeed *= MaxFallSpeedMultiplier;
            if (player.velocity.Y > 0f) {
                player.gravity *= FallGravityMultiplier;
                player.fallStart = (int)(player.position.Y / 16f);
            }
        }
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.Glowstick, GlowstickRecipeCount)
            .AddIngredient(ItemID.StickyGlowstick, GlowstickRecipeCount)
            .AddIngredient(ItemID.BouncyGlowstick, GlowstickRecipeCount)
            .AddIngredient(ItemID.FairyGlowstick, GlowstickRecipeCount)
            .AddIngredient(ItemID.SpelunkerGlowstick, GlowstickRecipeCount)
            .AddIngredient(ModContent.ItemType<BlackGlowstick.BlackGlowstick>(), GlowstickRecipeCount)
            .AddIngredient(ModContent.ItemType<CompressedTrash>(), 5)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}