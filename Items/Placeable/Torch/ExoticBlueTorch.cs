using AQMod.Dusts.NobleMushrooms;
using AQMod.Items.Placeable.CrabCrevice;
using AQMod.Items.Placeable.Nature;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Torch
{
    public class ExoticBlueTorch : ModItem
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
            item.noWet = true;
            item.flame = true;
            item.value = 50;
            item.placeStyle = Tiles.Furniture.Torches.ExoticBlueTorch;
        }

        public override void HoldItem(Player player)
        {
            if (item.wet)
            {
                return;
            }
            if (Main.rand.Next(player.itemAnimation > 0 ? 40 : 80) == 0)
            {
                var type = Main.rand.NextBool() ? ModContent.DustType<XenonDust>() : ModContent.DustType<XenonMist>();
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, type);
            }
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, 0f, 0f, 1f);
        }

        public override void PostUpdate()
        {
            if (!item.wet)
            {
                Lighting.AddLight((int)((item.position.X + item.width / 2) / 16f), (int)((item.position.Y + item.height / 2) / 16f), 1f, 1f, 2.5f);
            }
        }

        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            dryTorch = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.UltrabrightTorch, 20);
            r.AddIngredient(ModContent.ItemType<XenonMushroom>());
            r.SetResult(this, 20);
            r.AddRecipe();
        }
    }
}
