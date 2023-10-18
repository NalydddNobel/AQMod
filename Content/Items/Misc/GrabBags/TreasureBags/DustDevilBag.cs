using Aequus.Items.Misc.GrabBags.TreasureBags;
using Terraria.ID;

namespace Aequus.Content.Items.Misc.GrabBags.TreasureBags;

public class DustDevilBag : TreasureBagBase {
    protected override int InternalRarity => ItemRarityID.LightPurple;
    protected override bool PreHardmode => false;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.GetGlobalItem<AequusItem>().itemGravityCheck = 255;
    }
}