using AequusRemake.Content.Biomes.PollutedOcean;
using AequusRemake.Core.Entities.Tiles;
using Terraria.ObjectData;

namespace AequusRemake.Content.Tiles.PollutedOcean.Ambient.Pots;

public class TrashPots1x1 : BasePot {
    protected override void SetupTileObjectData() {
        base.SetupTileObjectData();
        TileObjectData.newTile.Width = 1;
        TileObjectData.newTile.Height = 1;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, };
        TileObjectData.newTile.Origin = new(0, 0);
        TileObjectData.newTile.DrawYOffset = 2;
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        HitSound = AequusSounds.TrashBag with { Pitch = 0.01f, PitchVariance = 0.05f };
    }

    protected override bool DoSpecialBiomeTorch(ref int itemID) {
        itemID = ModContent.GetInstance<PollutedOceanBiomeUnderground>().BiomeTorchItemType;
        return true;
    }

    protected override int ChooseGlowstick(int i, int j) {
        return ModContent.GetInstance<PollutedOceanBiomeUnderground>().BiomeTorchItemType;
    }
}