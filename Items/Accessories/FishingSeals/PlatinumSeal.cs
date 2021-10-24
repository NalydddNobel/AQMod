using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.FishingSeals
{
    public class PlatinumSeal : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.copperSeal = true;
            aQPlayer.silverSeal = true;
            aQPlayer.goldSeal = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<CopperSeal>());
            r.AddIngredient(ModContent.ItemType<SilverSeal>());
            r.AddIngredient(ModContent.ItemType<GoldSeal>());
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.TinkerersWorkbench);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}