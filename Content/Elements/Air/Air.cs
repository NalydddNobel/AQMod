using Aequus.Common.Bestiary;
using Aequus.Common.Elements;

namespace Aequus.Content.Elements.Air;

public class Air : Element {
    public Air() : base(Color.LightSkyBlue) { }

    public override void Load() {
        Air = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(BestiaryTags.SkySpace, BestiaryTags.NebulaPillar);
        AddItemRelationsFromNPCs();

        AddItem(ItemID.Amber);
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

        AddItemRelationsFromRecipes();
    }
}
