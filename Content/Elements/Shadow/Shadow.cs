using AequusRemake.Core.Entites.Bestiary;

namespace AequusRemake.Content.Elements.Shadow;

public class Shadow : VanillaElement {
    public Shadow() : base(ShadowFrame, Color.BlueViolet) { }

    public override void Load() {
        Shadow = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(
            BestiaryTags.GoblinInvaison,
            BestiaryTags.PumpkinMoon,
            BestiaryTags.Corruption,
            BestiaryTags.Crimson,
            BestiaryTags.CalamityBrimstoneCrags
        );

        AddNPC(NPCID.RedDevil);
        AddNPC("ThoriumMod/Viscount");

        AddItemRelationsFromNPCDrops();

        AddItem(ItemID.Amethyst);
        AddItem(ItemID.Deathweed);
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
        AddItem(ItemID.Bone);
        AddItem("ThoriumMod/Blood");
        AddItem("ThoriumMod/BloodFeasterStaff");

        RemoveItem(ItemID.SpikyBall);
        RemoveItem(ItemID.Harpoon);
        RemoveItem("ThoriumMod/IcyShard");

        AddItemRelationsFromRecipes();
    }
}
