using Terraria.ID;

namespace Aequus.Items.Misc.GrabBags.TreasureBags {
    public class CrabsonBag : TreasureBagBase {
        protected override int InternalRarity => ItemRarityID.Blue;
        protected override bool PreHardmode => true;
    }
}