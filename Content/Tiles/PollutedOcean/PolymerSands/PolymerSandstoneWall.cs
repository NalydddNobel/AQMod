#if POLLUTED_OCEAN
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Entities.Tiles;

namespace Aequus.Content.Tiles.PollutedOcean.PolymerSands;

#if POLLUTED_OCEAN_TODO
[LegacyName("SedimentaryRockWall", "PolymerSandWall")]
#endif
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

#if POLLUTED_OCEAN_TODO
[LegacyName("SedimentaryRockWallWall", "SedimentaryRockWallPlaced", "SedimentaryRockWallHostile", "PolymerSandWallHostile")]
#endif
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
#endif