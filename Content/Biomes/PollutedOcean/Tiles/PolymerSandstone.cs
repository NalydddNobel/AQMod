using Aequus.Common;
using Aequus.Common.Tiles;
using Aequus.Content.Biomes.PollutedOcean.Tiles.Scrap;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles;

[LegacyName("SedimentaryRockTile", "SedimentaryRock", "PolymerSand")]
public class PolymerSandstone : MultiMergeTile {
    public override void Load() {
        Mod.AddContent(new InstancedTileItem(this).WithRecipe((item) => {
            item.CreateRecipe(5)
                .AddIngredient(ItemID.SandBlock, 5)
                .AddIngredient(ScrapBlock.Item)
                .AddCondition(Condition.NearWater)
                .AddCondition(AequusConditions.InPollutedOcean)
                .Register();
        }));
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMerge(TileID.Sand);
        AddMerge(TileID.HardenedSand);
        
        TileID.Sets.ChecksForMerge[Type] = true;
        TileID.Sets.Conversion.Sandstone[Type] = true;
        AddMapEntry(new Color(160, 149, 97));
        DustType = DustID.Sand;
        HitSound = SoundID.Tink;
        MineResist = 1.5f;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}