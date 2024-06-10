using Aequus.Common.Bestiary;
using Aequus.Common.Elements;

namespace Aequus.Content.Elements.Water;

public class Water : Element {
    public Water() : base(Color.SkyBlue) { }

    public override void Load() {
        Water = this;
    }

    public override void SetupRelations() {
        AddBestiaryRelations(BestiaryTags.Ocean, BestiaryTags.Rain);

        AddNPC(NPCID.ZombieMerman);
        AddNPC(NPCID.EyeballFlyingFish);
        AddNPC(NPCID.BloodEelHead);
        AddNPC(NPCID.BloodEelBody);
        AddNPC(NPCID.BloodEelTail);
        AddNPC(NPCID.GoblinShark);
        AddNPC(NPCID.BloodNautilus);
        RemoveNPC(NPCID.IceGolem);

        AddItemRelationsFromNPCs();

        AddItem(ItemID.BottledWater);
        AddItem(ItemID.Muramasa);
        //AddItem(ItemID.Valor);
        AddItem(ItemID.WaterBolt);
        AddItem(ItemID.AquaScepter);
        AddItem(ItemID.MagicMissile);
        AddItem(ItemID.BlueMoon);
        //AddItem(ItemID.Handgun);
        AddItem(ItemID.CobaltShield);
        AddItem(ItemID.Trident);
        AddItem(ItemID.NeptunesShell);
        AddItem(ItemID.SharkFin);
        AddItem(ItemID.CrystalSerpent);
        AddItem(ItemID.Bladetongue);
        AddItem(ItemID.Toxikarp);
        AddItem(ItemID.Anchor);
        AddItem(ItemID.FalconBlade);

        AddItem(ItemID.Sapphire);

        AddItemRelationsFromRecipes();
    }
}
