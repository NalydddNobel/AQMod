using AequusRemake.Content.Configuration;

namespace AequusRemake.Systems.VanillaChanges;

internal class GravityGlobeChanges : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveGravityGlobe;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.type == ItemID.GravityGlobe;
    }

    public override void SetDefaults(Item entity) {
        entity.expert = false;
        entity.value = Item.buyPrice(gold: 5);
        entity.StatsModifiedBy.Add(Mod);
    }

    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (item.type == ItemID.MoonLordBossBag) {
            itemLoot.RemoveItemId(ItemID.GravityGlobe);
        }
    }
}

internal class GravityGlobeNPCChanges : GlobalNPC {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveGravityGlobe;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (npc.type == NPCID.MoonLordCore) {
            npcLoot.RemoveItemId(ItemID.GravityGlobe);
        }
    }
}
