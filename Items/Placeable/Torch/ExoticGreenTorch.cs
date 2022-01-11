using AQMod.Dusts.NobleMushrooms;
using AQMod.Items.Placeable.Nature;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Torch
{
    public class ExoticGreenTorch : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 12;
            item.maxStack = 999;
            item.holdStyle = 1;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.Furniture.Torches>();
            item.flame = true;
            item.value = 50;
            item.placeStyle = Tiles.Furniture.Torches.ExoticGreenTorch;
        }

        public override void HoldItem(Player player)
        {
            if (Main.rand.Next(player.itemAnimation > 0 ? 40 : 80) == 0)
            {
                var type = Main.rand.NextBool() ? ModContent.DustType<KryptonDust>() : ModContent.DustType<KryptonMist>();
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, type);
            }
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            float time = Main.GlobalTime * 2f;
            Lighting.AddLight(position, ((float)Math.Sin(time) + 1f) / 16f, ((float)Math.Cos(time) + 1f) / 4f, ((float)Math.Sin(time * 0.85f) + 1f) / 16f);
        }

        public override void PostUpdate()
        {
            float time = Main.GlobalTime * 2f;
            Lighting.AddLight((int)((item.position.X + item.width / 2) / 16f), (int)((item.position.Y + item.height / 2) / 16f), ((float)Math.Sin(time) + 1f) / 16f, ((float)Math.Cos(time) + 1f) / 4f, ((float)Math.Sin(time * 0.85f) + 1f) / 16f);
        }

        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            wetTorch = true;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Torch, 20);
            recipe.AddIngredient(ItemID.Emerald);
            recipe.AddIngredient(ModContent.ItemType<ExoticCoral>());
            recipe.SetResult(this, 20);
            recipe.AddRecipe();
        }
    }
}
