using Aequus.Common.Bestiary;
using Aequus.Common.Elements;

namespace Aequus.Content.Elements.Frost;

public class Frost : Element {
    public Frost() : base(Color.Cyan) { }

    public override void Load() {
        Frost = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(BestiaryTags.Snow, BestiaryTags.FrostLegion, BestiaryTags.FrostMoon, BestiaryTags.StardustPillar);
        AddItemRelationsFromNPCs();

        AddItem(ItemID.Sapphire);
        AddItem(ItemID.WandofFrosting);
        AddItem(ItemID.IceRod);
        AddItem(ItemID.StaffoftheFrostHydra);
        AddItem(ItemID.IceTorch);
        AddItem(ItemID.FrostDaggerfish);

        AddItemRelationsFromRecipes();
    }
}
