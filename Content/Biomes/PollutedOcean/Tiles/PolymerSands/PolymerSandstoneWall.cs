using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Components;
using Aequus.Core.ContentGeneration;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.PolymerSands;

[LegacyName("SedimentaryRockWall", "PolymerSandWall")]
public class PolymerSandstoneWall : ModWall, IWaterVisibleWall {
    public int WaterMapEntry { get; set; }

    public override void Load() {
        Mod.AddContent(new InstancedWallItem(this, dropItem: true));
    }

    public override void SetStaticDefaults() {
        Main.wallHouse[Type] = true;
        DustType = 209;

        Color mapColor = new Color(70, 40, 20);
        AddMapEntry(mapColor);
        WaterVisibleWall.CreateWaterEntry(this, mapColor);
    }
}

[LegacyName("SedimentaryRockWallWall", "SedimentaryRockWallPlaced", "SedimentaryRockWallHostile", "PolymerSandWallHostile")]
public class PolymerSandstoneWallHostile : PolymerSandstoneWall {
    public override string Texture => ModContent.GetInstance<PolymerSandstoneWall>().Texture;

    public override void Load() {
        Mod.AddContent(new InstancedWallItem(this, dropItem: false));
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        Main.wallHouse[Type] = false;
    }
}