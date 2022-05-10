﻿using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public sealed class CommonRecipes
    {
        public static void SpaceSquidDrop(ModItem modItem, int original)
        {
            modItem.CreateRecipe()
                .AddIngredient(original)
                .AddIngredient(ModContent.ItemType<AtmosphericEnergy>())
                .AddIngredient(ModContent.ItemType<FrozenTear>(), 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public static void RedSpriteDrop(ModItem modItem, int original, int amt = 1)
        {
            modItem.CreateRecipe()
                .AddIngredient(original, amt)
                .AddIngredient(ModContent.ItemType<AtmosphericEnergy>())
                .AddIngredient(ModContent.ItemType<Fluorescence>(), 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}