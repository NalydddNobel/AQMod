using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Summon
{
    public class Dreadsoul : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(gold: 10);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255 - item.alpha);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().dreadsoul = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Ectoplasm, 15);
            r.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 5);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}