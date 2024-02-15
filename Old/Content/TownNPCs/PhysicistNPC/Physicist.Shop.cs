using Aequus.Content.Tools.MagicMirrors.PhaseMirror;
using Aequus.Old.Content.Tools;

namespace Aequus.Old.Content.TownNPCs.PhysicistNPC;

public partial class Physicist {
    public override void AddShops() {
        new NPCShop(Type)
            .Add<PhaseMirror>()
            .Add<PhysicsGun>()
        //    .Add(ItemID.PortalGun, GameplayConfig.ConditionEarlyPortalGun)
        //    .Add(ItemID.GravityGlobe, GameplayConfig.ConditionEarlyGravityGlobe)
        //    .Add<LaserReticle>()
        //    .Add<HaltingMachine>()
        //    .Add<HolographicMeatloaf>(Condition.NotDontStarveWorld)
        //    .AddWithCustomValue(ItemID.BloodMoonStarter, Item.buyPrice(gold: 2))
        //    .Add<GalacticStarfruit>()
        //    .AddWithCustomValue(ItemID.SolarTablet, Item.buyPrice(gold: 5), Condition.DownedPlantera)
        //    .Add<PylonGunnerItem>()
        //    .Add<PylonHealerItem>()
        //    .Add<PylonCleanserItem>(Condition.NpcIsPresent(NPCID.Steampunker), Condition.NotRemixWorld)
        //    .Add<PhysicistSentry>(Condition.NotRemixWorld)
        //    .Add<Sentry6502>(Condition.RemixWorld)
        //    .Add<AntiGravityBlock>(Condition.NotZenithWorld)
        //    .Add<GravityBlock>(Condition.NotZenithWorld)
        //    .Add<AntiGravityBlock>(Condition.ZenithWorld, Condition.DownedEowOrBoc)
        //    .Add<GravityBlock>(Condition.ZenithWorld, Condition.DownedEowOrBoc)
        //    .Add<PhysicsBlock>()
        //    .Add<EmancipationGrill>()
        //    .Add<SupernovaFruit>(AequusConditions.DownedOmegaStarite)
        //    .Add<Tiles.Paintings.Canvas3x3.ExLydSpacePainting>(Condition.NpcIsPresent(NPCID.Painter))
        //    .Add<Tiles.Paintings.Canvas6x4.HomeworldPainting>(Condition.NpcIsPresent(NPCID.Painter))
        //    .Add<Tiles.Paintings.Canvas6x4.OmegaStaritePainting>(Condition.NpcIsPresent(NPCID.Painter))
        //    .Add<Tiles.Paintings.Canvas3x3.OmegaStaritePainting2>(Condition.NpcIsPresent(NPCID.Painter))
            .Register();

        //shop.item[nextSlot].SetDefaults(ModContent.ItemType<Cosmicanon>());
        //nextSlot++;
        //shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Transistor>());
        //if (Main.hardMode && NPC.downedMechBossAny)
        //{
        //    shop.item[nextSlot].SetDefaults(ModContent.ItemType<EclipseGlasses>());
        //    nextSlot++;
        //}

        //shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Stardrop>());
    }
}