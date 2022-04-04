using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Recipes
{
    public sealed class AequusRecipes
    {
        public static void SpaceSquidDrop(ModItem modItem, int original)
        {
            modItem.CreateRecipe()
                .AddIngredient(original)
                .AddIngredient(ModContent.ItemType<AtmosphericEnergy>())
                .AddIngredient(ModContent.ItemType<SiphonTentacle>(), 12)
                .AddIngredient(ItemID.SoulofFlight, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public static void RedSpriteDrop(ModItem modItem, int original)
        {
            modItem.CreateRecipe()
                .AddIngredient(original)
                .AddIngredient(ModContent.ItemType<AtmosphericEnergy>())
                .AddIngredient(ModContent.ItemType<Fluorescence>(), 12)
                .AddIngredient(ItemID.SoulofFlight, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
