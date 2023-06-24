using Terraria;

namespace Aequus.Content.UI.PyramidOffering {
    public class PyramidSelectionIcon {
        public OfferingInfo Offering;
        public Item ItemInstance;
        public int shake;
        public float failedOfferAnimation;

        public PyramidSelectionIcon(OfferingInfo offering) {
            Offering = offering;
            ItemInstance = new(offering.ItemID);
        }

        public void Update() {
            if (shake > 0) {
                shake--;
            }
            if (failedOfferAnimation > 0f) {
                failedOfferAnimation *= 0.9f;
                failedOfferAnimation -= 0.01f;
            }
        }
    }
}