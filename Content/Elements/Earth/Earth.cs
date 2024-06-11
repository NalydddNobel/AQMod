using Aequus.Common.Bestiary;
using Aequus.Common.Elements;

namespace Aequus.Content.Elements.Earth;

public class Earth : Element {
    public Earth() : base(Color.ForestGreen) { }

    public override void Load() {
        Earth = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(BestiaryTags.Jungle, BestiaryTags.GlowingMushroom, BestiaryTags.VortexPillar);

        AddItemRelationsFromNPCs();

        AddItem(ItemID.Emerald);
        AddItem(ItemID.Hive);
        AddItem(ItemID.HoneyBlock);
        AddItem(ItemID.BottledHoney);
        AddItem(ItemID.JungleSpores);
        AddItem(ItemID.DirtBlock);
        AddItem(ItemID.GrassSeeds);
        AddItem(ItemID.GlowingMushroom);
        AddItem(ItemID.AnkletoftheWind);
        AddItem(ItemID.FeralClaws);
        AddItem(ItemID.StaffofRegrowth);
        AddItem(ItemID.NaturesGift);
        AddItem(ItemID.JungleRose);
        AddItem(ItemID.ChlorophyteOre);
        AddItem("ThoriumMod/DeathEssence");

        RemoveItem(ItemID.Uzi);
        RemoveItem(ItemID.HeatRay);
        RemoveItem(ItemID.GrenadeLauncher);

        AddItemRelationsFromRecipes();
    }
}
