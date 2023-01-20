﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Materials
{
    public class Fluorescence : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 6));
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.SoulOfFlight;
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.rare = ItemDefaults.RarityGaleStreams - 1;
            Item.value = Item.sellPrice(silver: 15);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public static Recipe UpgradeItemRecipe(ModItem modItem, int original, int itemAmt = 1, bool sort = true)
        {
            return modItem.CreateRecipe()
                .AddIngredient(original)
                .AddIngredient<Fluorescence>(12)
                .AddTile(TileID.Anvils)
                .UnsafeSortRegister((r) =>
                {
                    if (sort)
                    {
                        r.SortAfterFirstRecipesOf(ItemID.RainbowRod);
                    }
                });
        }
    }
}