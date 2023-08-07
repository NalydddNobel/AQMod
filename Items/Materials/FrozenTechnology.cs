using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials {
    public class FrozenTechnology : ModItem {
        public override void SetStaticDefaults() {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.FrozenTechnology;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults() {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 80);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void AddRecipes() {
            Recipe.Create(ItemID.Compass)
                .AddIngredient(ItemID.CopperBar, 6)
                .AddIngredient(Type)
                .AddTile(TileID.Tables)
                .AddTile(TileID.Chairs)
                .Register()
                .DisableDecraft()
                .Clone()
                .ReplaceItem(ItemID.CopperBar, ItemID.TinBar)
                .Register();

            Recipe.Create(ItemID.DepthMeter)
                .AddIngredient(ItemID.SilverBar, 8)
                .AddIngredient(Type)
                .AddTile(TileID.Tables)
                .AddTile(TileID.Chairs)
                .Register()
                .DisableDecraft()
                .Clone()
                .ReplaceItem(ItemID.SilverBar, ItemID.TungstenBar)
                .Register();

            Recipe.Create(ItemID.LifeformAnalyzer)
                .AddIngredient(ItemID.GoldBar, 10)
                .AddIngredient(Type, 2)
                .AddTile(TileID.Tables)
                .AddTile(TileID.Chairs)
                .Register()
                .DisableDecraft()
                .Clone()
                .ReplaceItem(ItemID.GoldBar, ItemID.PlatinumBar)
                .Register();

            Recipe.Create(ItemID.MetalDetector)
                .AddIngredient(ItemID.DemoniteBar, 12)
                .AddIngredient(Type, 2)
                .AddTile(TileID.Tables)
                .AddTile(TileID.Chairs)
                .Register()
                .DisableDecraft()
                .Clone()
                .ReplaceItem(ItemID.DemoniteBar, ItemID.CrimtaneBar)
                .Register();

            Recipe.Create(ItemID.TallyCounter)
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(Type, 2)
                .AddTile(TileID.Tables)
                .AddTile(TileID.Chairs)
                .Register()
                .DisableDecraft()
                .Clone()
                .ReplaceItem(ItemID.DemoniteBar, ItemID.CrimtaneBar)
                .Register();

            Recipe.Create(ItemID.Stopwatch)
                .AddIngredient(ItemID.SilverBar, 6)
                .AddIngredient(Type)
                .AddTile(TileID.Tables)
                .AddTile(TileID.Chairs)
                .Register()
                .DisableDecraft()
                .Clone()
                .ReplaceItem(ItemID.SilverBar, ItemID.TungstenBar)
                .Register();

            Recipe.Create(ItemID.DPSMeter)
                .AddIngredient(ItemID.SilverBar, 10)
                .AddIngredient(Type)
                .AddTile(TileID.Tables)
                .AddTile(TileID.Chairs)
                .Register()
                .DisableDecraft()
                .Clone()
                .ReplaceItem(ItemID.SilverBar, ItemID.TungstenBar)
                .Register();

            Recipe.Create(ItemID.Radar)
                .AddIngredient(ItemID.CopperBar, 10)
                .AddIngredient(Type)
                .AddTile(TileID.Tables)
                .AddTile(TileID.Chairs)
                .Register()
                .DisableDecraft()
                .Clone()
                .ReplaceItem(ItemID.CopperBar, ItemID.TinBar)
                .Register();

            //Recipe.Create(ItemID.FishermansGuide)
            //    .AddIngredient(ItemID.IronBar, 12)
            //    .AddIngredient(Type)
            //    .AddTile(TileID.Tables)
            //    .AddTile(TileID.Chairs)
            //    .Register()
            //    .DisableDecraft()
            //    .Clone()
            //    .ReplaceItem(ItemID.IronBar, ItemID.LeadBar)
            //    .Register();

            //Recipe.Create(ItemID.WeatherRadio)
            //    .AddIngredient(ItemID.IronBar, 16)
            //    .AddIngredient(Type)
            //    .AddTile(TileID.Tables)
            //    .AddTile(TileID.Chairs)
            //    .Register()
            //    .DisableDecraft()
            //    .Clone()
            //    .ReplaceItem(ItemID.IronBar, ItemID.LeadBar)
            //    .Register();

            //Recipe.Create(ItemID.Sextant)
            //    .AddIngredient(ItemID.GoldBar, 8)
            //    .AddIngredient(Type)
            //    .AddTile(TileID.Tables)
            //    .AddTile(TileID.Chairs)
            //    .Register()
            //    .DisableDecraft()
            //    .Clone()
            //    .ReplaceItem(ItemID.GoldBar, ItemID.PlatinumBar)
            //    .Register();
        }
    }
}