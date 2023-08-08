using Aequus.Common.Items;
using Aequus.Common.Recipes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.GaleStreams {
    public class Fluorescence : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.SoulOfFlight;
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 6));
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<FrozenTear>());
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
            return Color.White;
        }
    }
}