namespace Aequus.Content.Bosses.TreasureBags;

public class CrabsonBag : TreasureBagBase {
    protected override int InternalRarity => ItemRarityID.Blue;
    protected override bool PreHardmode => true;
}