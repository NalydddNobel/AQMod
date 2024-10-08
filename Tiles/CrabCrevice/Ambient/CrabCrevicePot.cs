#if !CRAB_CREVICE_DISABLE
using Aequus.Common.Tiles;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace Aequus.Tiles.CrabCrevice.Ambient;
public class CrabCrevicePot : LegacyPotBase {
    public override void SetStaticDefaults() {
        TileObjectData.newTile.RandomStyleRange = 6;
        base.SetStaticDefaults();
        DustType = DustID.Pot;
    }

    public override void ModifyPotionDrop(ref int itemID, ref int stack, UnifiedRandom rng) {
        if (itemID == ItemID.RecallPotion && !rng.NextBool(3)) {
            itemID = ModContent.ItemType<Content.Items.Potions.SpawnpointPotion.SpawnpointPotion>();
        }
    }

    public override bool ChooseSpecialBiomeTorch(ref int itemID) {
        itemID = ItemID.CoralTorch;
        return true;
    }

    public override int ChooseGlowstick(int i, int j) {
        return ItemID.CoralTorch;
    }
}
#endif