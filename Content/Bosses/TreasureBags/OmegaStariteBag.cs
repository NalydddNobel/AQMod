namespace Aequus.Content.Bosses.TreasureBags;

public class OmegaStariteBag : TreasureBagBase {
    protected override int InternalRarity => ItemRarityID.LightRed;
    protected override bool PreHardmode => true;
}