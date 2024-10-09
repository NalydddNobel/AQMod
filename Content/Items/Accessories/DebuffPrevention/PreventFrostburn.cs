
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Items.Accessories.DebuffPrevention;

public class PreventFrostburn : ModItem {
    public static readonly int DropChance = 50;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.CloneShopValues(ItemID.ArmorPolish);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.buffImmune[BuffID.Frostburn] = true;
        player.buffImmune[BuffID.Frostburn2] = true;
    }

    [Gen.AequusNPC_ModifyNPCLoot]
    internal static void AddLoot(NPC npc, NPCLoot loot) {
        if (npc.type == NPCID.IceElemental) {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<PreventFrostburn>(), DropChance));
        }
    }
}
