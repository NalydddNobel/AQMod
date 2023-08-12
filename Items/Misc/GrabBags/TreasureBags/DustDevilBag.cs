using Terraria.ID;

namespace Aequus.Items.Misc.GrabBags.TreasureBags {
    public class DustDevilBag : TreasureBagBase {
        protected override int InternalRarity => ItemRarityID.LightPurple;
        protected override bool PreHardmode => true;

        public override void SetDefaults() {
            base.SetDefaults();
            Item.Aequus().itemGravityCheck = 255;
        }
    }
}