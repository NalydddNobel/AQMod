using AQMod.Common;
using AQMod.Common.CursorDyes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.CursorDyes
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
            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(gold: 1);
            item.maxStack = 999;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<AQVisualsPlayer>().CursorDyeID != CursorDyeManager.ID.None;
        }

        public override bool UseItem(Player player)
        {
            player.GetModPlayer<AQVisualsPlayer>().SetCursorDye(CursorDyeManager.ID.None);
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Bottle);
            r.AddRecipeGroup(AQMod.AnyEnergy);
            r.AddTile(TileID.DyeVat);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}