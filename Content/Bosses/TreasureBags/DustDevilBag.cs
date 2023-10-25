using Terraria.ID;

namespace Aequus.Content.Bosses.TreasureBags;

public class DustDevilBag : TreasureBagBase {
    protected override int InternalRarity => ItemRarityID.LightPurple;
    protected override bool PreHardmode => false;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.GetGlobalItem<AequusItem>().itemGravityCheck = 255;
    }
}