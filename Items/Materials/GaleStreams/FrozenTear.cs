using Aequus.Common.Items;
using Aequus.Common.Recipes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.GaleStreams {
    public class FrozenTear : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.SoulOfFlight;
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<Fluorescence>());
        }

        public override void SetDefaults() {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemDefaults.RarityGaleStreams - 1;
            Item.value = Item.sellPrice(silver: 15);
            Item.Aequus().itemGravityCheck = 255;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.Lerp(lightColor, Color.White, Helper.Wave(Item.timeSinceItemSpawned / 30f, 0.1f, 0.6f));
        }
    }
}