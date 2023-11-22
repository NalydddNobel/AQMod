using Aequus.Common.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles;

[LegacyName("SedimentaryRockWallWall", "SedimentaryRockWallPlaced")]
public class SedimentaryRockWallHostile : ModWall {
    public override string Texture => ModContent.GetInstance<SedimentaryRockWall>().Texture;

    public override void Load() {
        Mod.AddContent(new InstancedWallItem(this, dropItem: false));
    }

    public override void SetStaticDefaults() {
        Main.wallHouse[Type] = false;
        DustType = 209;
        AddMapEntry(new(70, 40, 20));
    }
}