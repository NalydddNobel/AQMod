using Aequus.Core.Entites.Bestiary;

namespace Aequus.Content.Elements.Water;

public class Water : VanillaElement {
    public Water() : base(WaterFrame, Color.SkyBlue) { }

    public override void Load() {
        Water = this;
    }

    protected override void SetupRelations() {
        AddBestiaryRelations(
            BestiaryTags.Ocean,
            BestiaryTags.Rain,
            BestiaryTags.CalamitySunkenSea,
            BestiaryTags.CalamitySulphurSea,
            BestiaryTags.CalamityAbyss,
            BestiaryTags.CalamityAcidRain,
            BestiaryTags.ThoriumAquaticDepths
        );

        AddNPC(NPCID.ZombieMerman);
        AddNPC(NPCID.EyeballFlyingFish);
        AddNPC(NPCID.BloodEelHead);
        AddNPC(NPCID.BloodEelBody);
        AddNPC(NPCID.BloodEelTail);
        AddNPC(NPCID.GoblinShark);
        AddNPC(NPCID.BloodNautilus);
        RemoveNPC(NPCID.IceGolem);

        AddItemRelationsFromNPCDrops();

        AddItem(ItemID.Topaz);
        AddItem(ItemID.Waterleaf);
        AddItem(ItemID.Coral);
        AddItem(ItemID.Starfish);
        AddItem(ItemID.Seashell);
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
        AddItem("ThoriumMod/WhirlpoolSaber");
        AddItem("ThoriumMod/NaiadShiv");
        AddItem("ThoriumMod/OceanEssence");

        AddItemRelationsFromRecipes();
    }
}
