using AQMod.Common.ID;
using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Dyes.Cursor
{
    public class CursorDyeRemover : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.buyPrice(gold: 1);
            item.maxStack = 999;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<PlayerCursorDyes>().cursorDye != CursorDyeID.None;
        }

        public override bool UseItem(Player player)
        {
            player.GetModPlayer<PlayerCursorDyes>().cursorDye = CursorDyeID.None;
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Bottle);
            r.AddRecipeGroup(AQRecipes.RecipeGroups.AnyEnergy);
            r.AddTile(TileID.DyeVat);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}