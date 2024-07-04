using AequusRemake.Core.Entites.Bestiary;

namespace AequusRemake.Content.Elements.Earth;

public class Earth : VanillaElement {
    public Earth() : base(EarthFrame, Color.ForestGreen) { }

    public override void Load() {
        Earth = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(BestiaryTags.Jungle, BestiaryTags.GlowingMushroom, BestiaryTags.VortexPillar);

        AddItemRelationsFromNPCDrops();

        AddItem(ItemID.Emerald);
        AddItem(ItemID.Blinkroot);
        AddItem(ItemID.Daybloom);
        AddItem(ItemID.Shiverthorn);
        AddItem(ItemID.Deathweed);
        AddItem(ItemID.Fireblossom);
        AddItem(ItemID.Moonglow);
        AddItem(ItemID.Waterleaf);
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
        AddItem("CalamityMod/PerennialOre");
        AddItem("CalamityMod/UelibloomOre");

        RemoveItem(ItemID.Uzi);
        RemoveItem(ItemID.HeatRay);
        RemoveItem(ItemID.GrenadeLauncher);

        AddItemRelationsFromRecipes();
    }
}
