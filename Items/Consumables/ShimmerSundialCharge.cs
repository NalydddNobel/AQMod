using Aequus.Common.Recipes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables {
    public class ShimmerSundialCharge : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults() {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.width = 16;
            Item.height = 16;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(silver: 10);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool? UseItem(Player player) {
            if (Main.myPlayer != player.whoAmI) {
                return false;
            }

            int x = Player.tileTargetX;
            int y = Player.tileTargetY;
            if (!WorldGen.InWorld(x, y, 40) || !player.IsInTileInteractionRange(x, y)) {
                return false;
            }

            var tile = Main.tile[x, y];
            if (tile.TileType == TileID.Sundial && Main.sundialCooldown > 0) {
                Main.sundialCooldown = 0;
                SoundEngine.PlaySound(SoundID.Item4, new Vector2(x, y) * 16f);
                return true;
            }
            if (tile.TileType == TileID.Moondial && Main.moondialCooldown > 0) {
                Main.moondialCooldown = 0;
                SoundEngine.PlaySound(SoundID.Item4, new Vector2(x, y) * 16f);
                return true;
            }
            return false;
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(ItemID.GoldWatch, Type);
            AequusRecipes.AddShimmerCraft(ItemID.PlatinumWatch, Type);
        }
    }
}