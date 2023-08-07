using Terraria.ID;

namespace Aequus.Items.Consumables.TreasureBag {
    public class OmegaStariteBag : TreasureBagBase {
        protected override int InternalRarity => ItemRarityID.LightRed;
        protected override bool PreHardmode => true;
    }
}