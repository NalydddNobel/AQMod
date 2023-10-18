using Aequus.Items.Misc.GrabBags.TreasureBags;
using Terraria.ID;

namespace Aequus.Content.Items.Misc.GrabBags.TreasureBags;

public class OmegaStariteBag : TreasureBagBase {
    protected override int InternalRarity => ItemRarityID.LightRed;
    protected override bool PreHardmode => true;
}