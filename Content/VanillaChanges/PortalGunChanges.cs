using AequusRemake.Content.Configuration;

namespace AequusRemake.Content.VanillaChanges;
internal class PortalGunChanges : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MovePortalGun;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.type == ItemID.PortalGun;
    }

    public override void SetDefaults(Item entity) {
        entity.expert = false;
        entity.value = Item.buyPrice(gold: 10);
        entity.StatsModifiedBy.Add(Mod);
    }

    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (item.type == ItemID.MoonLordBossBag) {
            itemLoot.RemoveItemId(ItemID.PortalGun);
        }
    }
}

internal class PortalGunNPCChanges : GlobalNPC {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MovePortalGun;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (npc.type == NPCID.MoonLordCore) {
            npcLoot.RemoveItemId(ItemID.PortalGun);
        }
    }
}
