using Aequus.Common.Bestiary;
using Aequus.Common.Elements;

namespace Aequus.Content.Elements.Shadow;

public class Shadow : Element {
    public Shadow() : base(Color.BlueViolet) { }

    public override void Load() {
        Shadow = this;
    }

    public override void SetupRelations() {
        AddBestiaryRelations(BestiaryTags.GoblinInvaison, BestiaryTags.PumpkinMoon, BestiaryTags.Corruption, BestiaryTags.Crimson);

        AddNPC(NPCID.RedDevil);

        AddItemRelationsFromNPCs();

        AddItem(ItemID.BallOHurt);
        AddItem(ItemID.BandofStarpower);
        AddItem(ItemID.Vilethorn);
        AddItem(ItemID.Musket);
        AddItem(ItemID.ShadowOrb);
        AddItem(ItemID.TheRottedFork);
        AddItem(ItemID.PanicNecklace);
        AddItem(ItemID.CrimsonRod);
        AddItem(ItemID.TheUndertaker);
        AddItem(ItemID.CrimsonHeart);
        AddItem(ItemID.ScourgeoftheCorruptor);
        AddItem(ItemID.VampireKnives);
        AddItem(ItemID.CorruptSeeds);
        AddItem(ItemID.CrimsonSeeds);
        AddItem(ItemID.SoulofNight);
        AddItem(ItemID.DarkLance);
        AddItem(ItemID.MoonCharm);
        AddItem(ItemID.MoonStone);
        AddItem(ItemID.ShadowbeamStaff);
        AddItem(ItemID.Bladetongue);
        AddItem(ItemID.Toxikarp);
        AddItem(ItemID.ShadowJoustingLance);

        AddItem(ItemID.Amethyst);

        RemoveItem(ItemID.SpikyBall);
        RemoveItem(ItemID.Harpoon);

        AddItemRelationsFromRecipes();
    }
}
