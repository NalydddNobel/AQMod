using AequusRemake.Core.Entites.Bestiary;

namespace AequusRemake.Systems.Elements.Frost;

public class Frost : VanillaElement {
    public Frost() : base(FrostFrame, Color.Cyan) { }

    public override void Load() {
        Frost = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(BestiaryTags.Snow, BestiaryTags.FrostLegion, BestiaryTags.FrostMoon, BestiaryTags.StardustPillar);
        AddItemRelationsFromNPCDrops();

        AddItem(ItemID.Sapphire);
        AddItem(ItemID.Shiverthorn);
        AddItem(ItemID.WandofFrosting);
        AddItem(ItemID.IceRod);
        AddItem(ItemID.StaffoftheFrostHydra);
        AddItem(ItemID.IceTorch);
        AddItem(ItemID.FrostDaggerfish);

        AddItemRelationsFromRecipes();
    }
}
