using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Items;
using Aequus.Items.Materials.PearlShards;
using Aequus.Tiles.CrabCrevice.Ambient;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice {
    [LegacyName("SedimentaryRock")]
    public class SedimentaryRockItem : FancyBlockItemBase<SedimentaryRockTile> {
        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.SandBlock)
                .AddIngredient(ItemID.StoneBlock)
                .AddCondition(Condition.InBeach)
                .AddCondition(Condition.NearWater)
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
            CreateRecipe()
                .AddIngredient<SedimentaryRockWallItem>(4)
                .AddTile(TileID.WorkBenches)
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
        }
    }

    public class SedimentaryRockTile : ModTile {
        public static int BiomeCount;

        public override void SetStaticDefaults() {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][TileID.Sand] = true;
            Main.tileMerge[Type][TileID.HardenedSand] = true;
            Main.tileMerge[TileID.Sand][Type] = true;
            Main.tileMerge[TileID.HardenedSand][Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.Conversion.Sandstone[Type] = true;
            AddMapEntry(new Color(160, 149, 97));
            DustType = DustID.Sand;
            HitSound = SoundID.Tink;
            MineResist = 1.25f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
            //TileFramingHelper.MergeWithFrame(i, j, Type, TileID.Sand);
            return true;
        }

        public static void RandomUpdateTile(int i, int j) {
            SeaPickleTile.TryGrow(i, j);
            CrabCreviceGenerator.GrowFloorTiles(i, j);
            CrabHydrosailia.TryGrow(i, j);
        }

        public override void RandomUpdate(int i, int j) {
            RandomUpdateTile(i, j);
        }
    }
}