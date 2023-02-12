using Terraria.ID;
using Terraria.ObjectData;

namespace Aequus.Tiles.CrabCrevice
{
    public class CrabCrevicePot : PotBase
    {
        public override void SetStaticDefaults()
        {
            TileObjectData.newTile.RandomStyleRange = 6;
            base.SetStaticDefaults();
            DustType = DustID.Pot;
        }

        public override bool DoSpecialBiomeTorch(ref int itemID)
        {
            itemID = ItemID.CoralTorch;
            return true;
        }

        public override int ChooseGlowstick(int i, int j)
        {
            return ItemID.CoralTorch;
        }
    }
}