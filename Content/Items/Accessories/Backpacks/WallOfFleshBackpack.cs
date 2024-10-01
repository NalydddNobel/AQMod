using Aequus.Common.Utilities;
using Aequus.Systems.Backpacks;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.Backpacks;

[AutoloadEquip(EquipType.Back)]
internal class WallOfFleshBackpack : BackpackModItem {
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Capacity);

    public override int Capacity { get; set; } = 30;
    public override float SlotHue { get; set; } = 0.33f;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.CloneShopValues(ItemID.LaserRifle);
    }

    [Gen.AequusNPC_ModifyNPCLoot]
    internal static void ModifyNPCDrops(NPC npc, NPCLoot loot) {
#if !DEBUG
        return;
#endif
        if (npc.type == NPCID.WallofFlesh) {
            LootBuilder.AddToGrabBag(ItemID.WallOfFleshBossBag, ItemDropRule.Common(ModContent.ItemType<WallOfFleshBackpack>(), chanceDenominator: 2));
            loot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<WallOfFleshBackpack>(), chanceDenominator: 4));
        }
    }

#if !DEBUG
    public override bool IsLoadingEnabled(Mod mod) {
        return false;
    }
#endif
}