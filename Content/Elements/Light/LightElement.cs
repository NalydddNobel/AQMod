using Aequus.Common.Bestiary;
using Aequus.Common.Elements;

namespace Aequus.Content.Elements.Light;

public class LightElement : Element {
    public LightElement() : base(Color.LightPink) { }

    public override void Load() {
        Light = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(
            BestiaryTags.Hallow,
            BestiaryTags.CelestialPillars,
            BestiaryTags.CalamityAstral
        );

        AddItemRelationsFromNPCs();

        AddItem(ItemID.Diamond);
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

        RemoveItem(ItemID.Sandgun);

        AddItemRelationsFromRecipes();
    }
}
