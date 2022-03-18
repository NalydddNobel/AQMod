using AQMod.Tiles.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials
{
    public class CrustaciumOre : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Blue;
            item.createTile = ModContent.TileType<CrustaciumShell>();
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<CrustaciumBlob>(), 2);
            r.AddTile(TileID.Furnaces);
            r.SetResult(this);
            r.AddRecipe();
        }

        public override void CaughtFishStack(ref int stack)
        {
            stack = Main.rand.Next(6) + 1;
        }
    }
}