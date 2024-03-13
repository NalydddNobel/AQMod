using Aequus.Common.Tiles;
using Terraria.ObjectData;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.Ambient.Pots;

public class TrashPots2x2 : BasePot {
    protected override void SetupTileObjectData() {
        base.SetupTileObjectData();
        TileObjectData.newTile.DrawYOffset = 2;
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        HitSound = SoundID.Grass;
    }

    protected override bool DoSpecialBiomeTorch(ref int itemID) {
        itemID = ModContent.GetInstance<PollutedOceanBiomeUnderground>().BiomeTorchItemType;
        return true;
    }

    protected override int ChooseGlowstick(int i, int j) {
        return ModContent.GetInstance<PollutedOceanBiomeUnderground>().BiomeTorchItemType;
    }
}