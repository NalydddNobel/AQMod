using Aequus.Common;
using Aequus.Common.Utilities;
using Aequus.Systems.Backpacks;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.Backpacks;

// Uses front sheeting temporarily, please add a unique backpack strap layer.
[AutoloadEquip(EquipType.Back, EquipType.Front)]
[WorkInProgress]
public class PlanteraBackpack : BackpackModItem {
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Capacity);

    public override int Capacity { get; set; } = 40;
    public override float SlotHue { get; set; } = 0.64f;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.CloneShopValues(ItemID.PygmyStaff);
    }

    [Gen.AequusNPC_ModifyNPCLoot]
    internal static void ModifyNPCDrops(NPC npc, NPCLoot loot) {
#if !DEBUG
        return;
#endif
        if (npc.type == NPCID.Plantera) {
            LootBuilder.AddToGrabBag(ItemID.PlanteraBossBag, ItemDropRule.Common(ModContent.ItemType<PlanteraBackpack>(), chanceDenominator: 2));
            loot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<PlanteraBackpack>(), chanceDenominator: 4));
        }
    }

#if !DEBUG
    public override bool IsLoadingEnabled(Mod mod) {
        return false;
    }
#endif
}