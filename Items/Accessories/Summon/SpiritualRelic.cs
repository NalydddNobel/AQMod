using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Summon
{
    public class SpiritualRelic : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = ItemRarityID.Cyan;
            item.value = Item.sellPrice(gold: 10);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255 - item.alpha);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.dreadsoul = true;
            aQPlayer.breadsoul = true;
            player.maxMinions++;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.PygmyNecklace);
            r.AddIngredient(ModContent.ItemType<Breadsoul>());
            r.AddIngredient(ModContent.ItemType<Dreadsoul>());
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.TinkerersWorkbench);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
