using Aequus.Content.Configuration;
using Aequus.Content.Events.Deactivators;
using Aequus.Old.Content.DronePylons;
using Aequus.Old.Content.Equipment.Accessories.LaserScope;
using Aequus.Old.Content.Events.Glimmer.Spawners;
using Aequus.Old.Content.Tiles.GravityBlocks;
using Aequus.Old.Content.Tools;
using Aequus.Old.Content.Weapons.Sentries.PhysicistSentry;

namespace Aequus.Old.Content.TownNPCs.PhysicistNPC;

public partial class Physicist {
    public override void AddShops() {
        new NPCShop(Type)
            .Add<PhysicsGun>()
            .Add(ItemID.PortalGun, new Condition("Mods.Aequus.Condition.MovePortalGun", () => VanillaChangesConfig.Instance.MovePortalGun))
            .Add(ItemID.GravityGlobe, new Condition("Mods.Aequus.Condition.MoveGravityGlobe", () => VanillaChangesConfig.Instance.MoveGravityGlobe))
            .Add<LaserReticle>()
        //    .Add<HaltingMachine>()
            .AddCustomValue(ItemID.BloodMoonStarter, Item.buyPrice(gold: 2))
            .Add<GalacticStarfruit>()
            .AddCustomValue(ItemID.SolarTablet, Item.buyPrice(gold: 5), Condition.DownedPlantera)
            .Add(ModContent.GetInstance<GunnerDroneSlot>().DroneItem.Type)
            .Add(ModContent.GetInstance<HealerDroneSlot>().DroneItem.Type)
            .Add(ModContent.GetInstance<CleanserDroneSlot>().DroneItem.Type, Condition.NpcIsPresent(NPCID.Steampunker), Condition.NotRemixWorld)
            .Add<PhysicistSentry>(Condition.NotRemixWorld)
            .Add(EventDeactivators.BloodMoonItem.Type, Condition.DownedPlantera)
            .Add(EventDeactivators.GlimmerItem.Type, Condition.DownedPlantera)
            .Add(EventDeactivators.EclipseItem.Type, Condition.DownedPlantera)
            .Add(GravityBlocks.NormalGravityBlockItem.Type)
            .Add(GravityBlocks.ReverseGravityBlockItem.Type)
        //    .Add<PhysicsBlock>()
        //    .Add<EmancipationGrill>()
        //    .Add<SupernovaFruit>(AequusConditions.DownedOmegaStarite)
        //    .Add<Tiles.Paintings.Canvas3x3.ExLydSpacePainting>(Condition.NpcIsPresent(NPCID.Painter))
        //    .Add<Tiles.Paintings.Canvas6x4.HomeworldPainting>(Condition.NpcIsPresent(NPCID.Painter))
        //    .Add<Tiles.Paintings.Canvas6x4.OmegaStaritePainting>(Condition.NpcIsPresent(NPCID.Painter))
        //    .Add<Tiles.Paintings.Canvas3x3.OmegaStaritePainting2>(Condition.NpcIsPresent(NPCID.Painter))
            .Register();

        //shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Stardrop>());
    }
}