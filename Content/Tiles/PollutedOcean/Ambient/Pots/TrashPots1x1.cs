using Aequus.Common.ContentTemplates;
using Aequus.Content.Biomes.PollutedOcean;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient.Pots;

public class TrashPots1x1 : UnifiedBreakablePot {
    protected override void SetupTileObjectData() {
        base.SetupTileObjectData();
        TileObjectData.newTile.Width = 1;
        TileObjectData.newTile.Height = 1;
        TileObjectData.newTile.CoordinateHeights = [16,];
        TileObjectData.newTile.Origin = new(0, 0);
        TileObjectData.newTile.DrawYOffset = 2;
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        HitSound = AequusSounds.TileBreak_TrashBag with { Pitch = 0.01f, PitchVariance = 0.05f };
    }

    protected override bool DoSpecialBiomeTorch(ref int itemID) {
        itemID = ModContent.GetInstance<PollutedOceanUnderground>().BiomeTorchItemType;
        return true;
    }

    protected override int ChooseGlowstick(int i, int j) {
        return ModContent.GetInstance<PollutedOceanUnderground>().BiomeTorchItemType;
    }
}