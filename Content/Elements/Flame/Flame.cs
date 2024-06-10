using Aequus.Common.Bestiary;
using Aequus.Common.Elements;

namespace Aequus.Content.Elements.Flame;

public class Flame : Element {
    public Flame() : base(Color.Orange) { }

    public override void Load() {
        Flame = this;
    }

    public override void SetupRelations() {
        AddBestiaryRelations(BestiaryTags.Underworld, BestiaryTags.PumpkinMoon, BestiaryTags.SolarPillar);

        RemoveNPC(NPCID.WallofFlesh);
        RemoveNPC(NPCID.WallofFleshEye);

        AddItemRelationsFromNPCs();

        AddItem(ItemID.FireWhip);
        AddItem(ItemID.WandofSparking);
        AddItem(ItemID.Torch);
        AddItem(ItemID.Hellstone);
        AddItem(ItemID.Meteorite);
        AddItem(ItemID.HeatRay);
        AddItem(ItemID.InfernoFork);
        AddItem(ItemID.Flamelash);
        AddItem(ItemID.Sunfury);
        AddItem(ItemID.Sunfury);

        AddItem(ItemID.Ruby);

        AddItemRelationsFromRecipes();
    }
}
