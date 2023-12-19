using Aequus.Content.Tiles.Radon;

namespace Aequus.Content.Fishing.Equipment.RadonFishingBobber;

public class RadonFishingBobber : ModItem {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.FishingBobberGlowingRainbow);
        Item.glowMask = GlowMaskID.None;
    }

    private void UpdateFishingBobber(Player player) {
        player.overrideFishingBobber = ModContent.ProjectileType<RadonFishingBobberProj>();
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.accFishingBobber = true;
        if (!hideVisual) {
            UpdateFishingBobber(player);
        }
        if (player.HeldItem.IsAir || player.HeldItem.fishingPole <= 0) {
            return;
        }
        for (int i = 0; i < Main.maxProjectiles; i++) {
            if (Main.projectile[i].active && Main.projectile[i].bobber && Main.projectile[i].owner == player.whoAmI) {
                player.AddBuff(ModContent.BuffType<RadonFishingBobberBuff>(), 8);
            }
        }
    }

    public override void UpdateVanity(Player player) {
        UpdateFishingBobber(player);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.FishingBobberGlowingStar)
            .AddIngredient(RadonMoss.Item, 5)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}