using Aequus.Core.Entites.Bestiary;

namespace Aequus.Content.Elements.Light;

public class LightElement : VanillaElement {
    public LightElement() : base(LightFrame, Color.LightPink) { }

    public override void Load() {
        Light = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(
            BestiaryTags.Hallow,
            BestiaryTags.CelestialPillars,
            BestiaryTags.CalamityAstral
        );

        AddItemRelationsFromNPCDrops();

        AddItem(ItemID.Diamond);
        AddItem(ItemID.Daybloom);
        AddItem(ItemID.HallowedSeeds);
        AddItem(ItemID.RainbowGun);
        AddItem(ItemID.CrystalShard);
        AddItem(ItemID.SoulofLight);
        AddItem(ItemID.HallowedBar);
        AddItem(ItemID.Pwnhammer);
        AddItem(ItemID.SunStone);
        AddItem(ItemID.BeamSword);
        AddItem(ItemID.EnchantedSword);
        AddItem(ItemID.CrystalSerpent);
        AddItem(ItemID.PrincessWeapon);
        AddItem("ThoriumMod/EnchantedCane");
        AddItem("CalamityMod/AuricOre");
        AddItem("CalamityMod/HallowedOre");
        AddItem("CalamityMod/AstralOre");

        RemoveItem(ItemID.Sandgun);

        AddItemRelationsFromRecipes();
    }
}
