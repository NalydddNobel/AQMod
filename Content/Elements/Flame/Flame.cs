using Aequu2.Core.Entites.Bestiary;

namespace Aequu2.Content.Elements.Flame;

public class Flame : VanillaElement {
    public Flame() : base(FlameFrame, Color.Orange) { }

    public override void Load() {
        Flame = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(BestiaryTags.Underworld, BestiaryTags.PumpkinMoon, BestiaryTags.SolarPillar);

        RemoveNPC(NPCID.WallofFlesh);
        RemoveNPC(NPCID.WallofFleshEye);

        AddItemRelationsFromNPCDrops();

        AddItem(ItemID.Ruby);
        AddItem(ItemID.Fireblossom);
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
        AddItem("ThoriumMod/SoulofPlight");
        AddItem("ThoriumMod/InfernoEssence");
        AddItem("ThoriumMod/DraconicMagmaStaff");
        AddItem("CalamityMod/ScoriaOre");

        RemoveItem(ItemID.WandofFrosting);
        RemoveItem(ItemID.IceTorch);
        RemoveItem(ItemID.GoldCrown);
        RemoveItem(ItemID.PlatinumCrown);

        AddItemRelationsFromRecipes();
    }
}
