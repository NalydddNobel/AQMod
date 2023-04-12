using Aequus.Common.Recipes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials {
    public class FrozenTear : ModItem {
        private float _gravity;

        public override void SetStaticDefaults() {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.SoulOfFlight;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults() {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.rare = ItemDefaults.RarityGaleStreams - 1;
            Item.value = Item.sellPrice(silver: 15);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.Lerp(lightColor, Color.White, Helper.Wave(Item.timeSinceItemSpawned / 30f, 0.1f, 0.6f));
        }

        public override void Update(ref float gravity, ref float maxFallSpeed) {
            if (gravity <= 0f) {
                return;
            }

            if (Item.timeSinceItemSpawned % 30 == 0) {
                _gravity = Helper.IsShimmerBelow(Item.Center.ToTileCoordinates(), 12) ? gravity : 0f;
            }
            if (_gravity == 0f) {
                Item.velocity.Y *= 0.95f;
            }
            gravity = _gravity;
        }

        public override void AddRecipes() {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<Fluorescence>());
        }

        public static Recipe UpgradeItemRecipe(ModItem modItem, int original, int itemAmt = 1, bool sort = true) {
            return modItem.CreateRecipe()
                .AddIngredient(original)
                .AddIngredient<FrozenTear>(12)
                .AddTile(TileID.Anvils)
                .UnsafeSortRegister((r) => {
                    if (sort) {
                        r.SortAfterFirstRecipesOf(ItemID.RainbowRod);
                    }
                });
        }
    }
}