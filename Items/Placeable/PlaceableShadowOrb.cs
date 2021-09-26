using AQMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable
{
    public class PlaceableShadowOrb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.useTime = 15;
            item.useAnimation = 15;
            item.consumable = true;
            item.placeStyle = 0;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.maxStack = 999;
        }

        public override void HoldItem(Player player)
        {
            if (player.CanReach_IgnoreItemTileBoost())
            {
                player.showItemIcon = true;
            }
        }

        public override bool UseItem(Player player)
        {
            if (player.CanReach_IgnoreItemTileBoost() && AQUtils.Check2x2ThenCut(Player.tileTargetX, Player.tileTargetY))
            {
                Main.PlaySound(SoundID.Dig, player.position);
                Main.tile[Player.tileTargetX, Player.tileTargetY].active(active: true);
                Main.tile[Player.tileTargetX, Player.tileTargetY].type = TileID.ShadowOrbs;
                Main.tile[Player.tileTargetX + 1, Player.tileTargetY].active(active: true);
                Main.tile[Player.tileTargetX + 1, Player.tileTargetY].type = TileID.ShadowOrbs;
                Main.tile[Player.tileTargetX, Player.tileTargetY + 1].active(active: true);
                Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type = TileID.ShadowOrbs;
                Main.tile[Player.tileTargetX + 1, Player.tileTargetY + 1].active(active: true);
                Main.tile[Player.tileTargetX + 1, Player.tileTargetY + 1].type = TileID.ShadowOrbs;
                Main.tile[Player.tileTargetX, Player.tileTargetY].frameX = 0;
                Main.tile[Player.tileTargetX, Player.tileTargetY].frameY = 0;
                Main.tile[Player.tileTargetX + 1, Player.tileTargetY].frameX = 18;
                Main.tile[Player.tileTargetX + 1, Player.tileTargetY].frameY = 0;
                Main.tile[Player.tileTargetX, Player.tileTargetY + 1].frameX = 0;
                Main.tile[Player.tileTargetX, Player.tileTargetY + 1].frameY = 18;
                Main.tile[Player.tileTargetX + 1, Player.tileTargetY + 1].frameX = 18;
                Main.tile[Player.tileTargetX + 1, Player.tileTargetY + 1].frameY = 18;
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ShadowScale);
            recipe.AddIngredient(ItemID.VilePowder, 5);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}