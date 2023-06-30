using Aequus.Common.Tiles;
using Aequus.Content.World;
using Aequus.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Misc.HardmodeChests {
    public class HardGraniteChest : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.GraniteChest;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardGraniteChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes() {
            CreateRecipe(5)
                .AddIngredient(ItemID.GraniteChest, 5)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class HardGraniteChestTile : BaseChest<HardGraniteChest> {
        public override Color MapColor => new(100, 255, 255);

        public override void SetStaticDefaults() {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Gold);
            ChestType.IsGenericUndergroundChest.Add(new(Type));
            base.SetStaticDefaults();
            DustType = DustID.Granite;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
            DrawBasicGlowmask(i, j, spriteBatch, AequusTextures.HardGraniteChestTile_Glow, Color.White);
        }
    }
}