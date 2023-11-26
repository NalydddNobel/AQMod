using Aequus.Common.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles;

[LegacyName("SedimentaryRockWallWall", "SedimentaryRockWallPlaced", "SedimentaryRockWallHostile")]
public class PolymerSandWallHostile : ModWall {
    public override string Texture => ModContent.GetInstance<PolymerSandWall>().Texture;

    public override void Load() {
        Mod.AddContent(new InstancedWallItem(this, dropItem: false));
    }

    public override void SetStaticDefaults() {
        Main.wallHouse[Type] = false;
        DustType = 209;
        AddMapEntry(new(70, 40, 20));
    }
}