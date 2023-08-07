using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials {
    public class GlowLichen : ModItem {
        public override void SetStaticDefaults() {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.DemoniteOre;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults() {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 2);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void PostUpdate() {
            Lighting.AddLight(Item.Center, new Vector3(0.4f, 0.4f, 0.4f));
        }

        public override void AddRecipes() {
            Recipe.Create(ItemID.UltrabrightTorch, 33)
                .AddIngredient(ItemID.Torch, 33)
                .AddIngredient(Type)
                .Register()
                .DisableDecraft();
        }
    }
}