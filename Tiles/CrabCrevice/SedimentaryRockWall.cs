using Aequus.Common.Recipes;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Items.Materials.PearlShards;
using Aequus.Tiles.CrabCrevice.Ambient;
using Aequus.Tiles.Misc.Herbs.Moray;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice {
    [LegacyName("SedimentaryRockWallWall")]
    public class SedimentaryRockWallPlaced : ModWall {
        public override void SetStaticDefaults() {
            DustType = 209;
            AddMapEntry(new Color(70, 40, 20));
        }

        public override void RandomUpdate(int i, int j) {
            if (AequusWorld.downedCrabson && WorldGen.genRand.NextBool(1600)) {
                MorayTile.TryGrow(i, j);
            }
            if (Main.tile[i, j].IsFullySolid()) {
                if (WorldGen.genRand.NextBool(2000)) {
                    PearlsTile.TryGrow(i, j);
                }
                if (WorldGen.genRand.NextBool(20)) {
                    SeaPickleTile.TryGrow(i, j);
                }
                if (WorldGen.genRand.NextBool(20)) {
                    CrabCreviceGenerator.GrowFloorTiles(i, j);
                }
            }
            if (WorldGen.genRand.NextBool(20)) {
                CrabHydrosailia.TryGrow(i, j);
            }
        }
    }

    [LegacyName("SedimentaryRockWallPlacedFriendly")]
    public class SedimentaryRockWallFriendlyPlaced : SedimentaryRockWallPlaced {
        public override string Texture => AequusTextures.SedimentaryRockWallPlaced.Path;
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            RegisterItemDrop(ModContent.ItemType<SedimentaryRockWallItem>());
            Main.wallHouse[Type] = true;
        }

        public override void RandomUpdate(int i, int j) {
        }
    }

    [LegacyName("SedimentaryRockWallWall")]
    public class SedimentaryRockWallItem : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<SedimentaryRockWallFriendlyPlaced>());
        }

        public override void AddRecipes() {
            CreateRecipe(4)
                .AddIngredient<SedimentaryRockItem>()
                .AddTile(TileID.WorkBenches)
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
        }
    }

    [LegacyName("SedimentaryRockWallHostile")]
    public class SedimentaryRockWallItemHostile : SedimentaryRockWallItem {
        public override string Texture => AequusTextures.SedimentaryRockWallItem.Path;

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<SedimentaryRockWallPlaced>());
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(ModContent.ItemType<SedimentaryRockWallItem>(), Type);
        }
    }
}