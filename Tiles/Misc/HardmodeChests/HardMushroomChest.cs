using Aequus.Content.WorldGeneration;
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

    public class HardMushroomChestTile : BaseChest {
        public override void SetStaticDefaults() {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Gold);
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ItemDrop = ModContent.ItemType<HardMushroomChest>();
            AddMapEntry(new Color(0, 50, 215), CreateMapEntryName());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
            DrawBasicGlowmask(i, j, spriteBatch);
        }
    }
}