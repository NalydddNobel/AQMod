using Aequus.Common.Tiles;
using Aequus.Content.Biomes.PollutedOcean;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient.Pots;

public class TrashPots2x2 : BasePot {
    protected override void SetupTileObjectData() {
        base.SetupTileObjectData();
        TileObjectData.newTile.DrawYOffset = 2;
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        HitSound = AequusSounds.TrashBag with { Pitch = -0.1f, PitchVariance = 0.06f };
    }

    protected override bool DoSpecialBiomeTorch(ref int itemID) {
        itemID = ModContent.GetInstance<PollutedOceanBiomeUnderground>().BiomeTorchItemType;
        return true;
    }

    protected override int ChooseGlowstick(int i, int j) {
        return ModContent.GetInstance<PollutedOceanBiomeUnderground>().BiomeTorchItemType;
    }
}