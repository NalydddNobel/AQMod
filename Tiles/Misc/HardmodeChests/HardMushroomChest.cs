using Aequus.Common.Tiles;
using Aequus.Content.World;
using Aequus.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Misc.HardmodeChests {
    public class HardMushroomChest : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MushroomChest;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardMushroomChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes() {
            CreateRecipe(5)
                .AddIngredient(ItemID.MushroomChest, 5)
                .AddIngredient(ItemID.ShroomiteBar, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class HardMushroomChestTile : BaseChest<HardMushroomChest> {
        public override Color MapColor => new(0, 50, 215);

        public override void SetStaticDefaults() {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Gold);
            ChestType.IsGenericUndergroundChest.Add(new(Type));
            base.SetStaticDefaults();
            DustType = DustID.GlowingMushroom;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
            DrawBasicGlowmask(i, j, spriteBatch, AequusTextures.HardMushroomChestTile_Glow, Color.White);
        }
    }
}