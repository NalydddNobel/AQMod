using Aequus.Common.Recipes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.CrabCrevice.Tiles {
    public class SedimentaryRockWall : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<SedimentaryRockWallPlacedFriendly>());
        }

        public override void AddRecipes() {
            CreateRecipe(4)
                .AddIngredient<SedimentaryRock>()
                .AddTile(TileID.WorkBenches)
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
        }
    }
    public class SedimentaryRockWallHostile : SedimentaryRockWall {
        public override string Texture => AequusTextures.SedimentaryRockWall.Path;

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<SedimentaryRockWallWall>());
        }

        public override void AddRecipes() {
            AequusRecipes.CreateShimmerTransmutation(ModContent.ItemType<SedimentaryRockWall>(), Type);
        }
    }
    public class SedimentaryRockWallWall : ModWall {
        public override void SetStaticDefaults() {
            DustType = 209;
            ItemDrop = ModContent.ItemType<SedimentaryRockWall>();
            AddMapEntry(new Color(70, 40, 20));
        }
    }
    public class SedimentaryRockWallPlacedFriendly : SedimentaryRockWallWall {
        public override string Texture => AequusTextures.SedimentaryRockWallWall.Path;
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = true;
        }
    }
}