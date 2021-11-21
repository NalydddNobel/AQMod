using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Torch
{
    public class UltrabrightRedTorch : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 12;
            item.maxStack = 999;
            item.holdStyle = 1;
            item.noWet = true;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.Torches>();
            item.value = 50;
            item.placeStyle = Tiles.Torches.UltrabrightRedTorch;
        }

        public override void HoldItem(Player player)
        {
            if (Main.rand.Next(player.itemAnimation > 0 ? 40 : 80) == 0)
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<XenonDust>());
            }
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, 1f, 0f, 0f);
        }

        public override void PostUpdate()
        {
            if (!item.wet)
            {
                Lighting.AddLight((int)((item.position.X + item.width / 2) / 16f), (int)((item.position.Y + item.height / 2) / 16f), 1f, 0f, 0f);
            }
        }

        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            dryTorch = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.UltrabrightTorch, 20);
            recipe.AddIngredient(ModContent.ItemType<ArgonMushroom>());
            recipe.SetResult(this, 20);
            recipe.AddRecipe();
        }
    }
}
