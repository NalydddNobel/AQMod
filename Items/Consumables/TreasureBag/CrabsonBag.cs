using Terraria.ID;

namespace Aequus.Items.Consumables.TreasureBag {
    public class CrabsonBag : TreasureBagBase {
        protected override int InternalRarity => ItemRarityID.Blue;
        protected override bool PreHardmode => true;
    }
}