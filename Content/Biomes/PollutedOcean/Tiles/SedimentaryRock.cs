using Aequus.Common;
using Aequus.Common.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles;

[LegacyName("SedimentaryRockTile")]
public class SedimentaryRock : AequusModTile {
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
        MineResist = 1.5f;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

    internal override void AddItemRecipes(AutoloadedTileItem modItem) {
        modItem.CreateRecipe()
            .AddIngredient(ItemID.SandBlock)
            .AddIngredient(ItemID.StoneBlock)
            .AddCondition(AequusConditions.InPollutedOcean);
    }
}