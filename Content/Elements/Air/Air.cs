using Aequus.Core.Entites.Bestiary;

namespace Aequus.Content.Elements.Air;

public class Air : VanillaElement {
    public Air() : base(AirFrame, Color.LightSkyBlue) { }

    public override void Load() {
        Air = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(BestiaryTags.SkySpace, BestiaryTags.NebulaPillar);
        AddItemRelationsFromNPCDrops();

        AddItem(ItemID.Amber);
        AddItem(ItemID.Blinkroot);
        AddItem(ItemID.Cloud);
        AddItem(ItemID.RainCloud);
        AddItem(ItemID.SnowCloudBlock);
        AddItem(ItemID.Starfury);
        AddItem(ItemID.ShinyRedBalloon);
        AddItem(ItemID.LuckyHorseshoe);
        AddItem(ItemID.CelestialMagnet);
        AddItem(ItemID.AnkletoftheWind);
        AddItem(ItemID.StormTigerStaff);
        AddItem(ItemID.StarWrath);
        AddItem(ItemID.SandBlock);
        AddItem(ItemID.AntlionMandible);
        AddItem(ItemID.FossilOre);
        AddItem(ItemID.SunplateBlock);

        AddItemRelationsFromRecipes();
    }
}
