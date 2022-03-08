using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class Extractor : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 1);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Suffocation] = true;
            player.pickSpeed *= 0.75f;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.extractorHelmet = 0;
            aQPlayer.extractorAirMask = 0;
            aQPlayer.reducedSiltDamage = true;
            aQPlayer.extractinatorCounter = !hideVisual;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<ExtractorAirMask>());
            r.AddIngredient(ModContent.ItemType<ExtractorHelmet>());
            r.AddTile(TileID.TinkerersWorkbench);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}