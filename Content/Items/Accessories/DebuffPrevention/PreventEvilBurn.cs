
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Items.Accessories.DebuffPrevention;

public class PreventEvilBurn : ModItem {
    public static readonly int DropChance = 50;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.CloneShopValues(ItemID.ArmorPolish);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.buffImmune[BuffID.CursedInferno] = true;
        player.buffImmune[BuffID.Ichor] = true;
    }

    [Gen.AequusNPC_ModifyNPCLoot]
    internal static void AddLoot(NPC npc, NPCLoot loot) {
        if (npc.type == NPCID.Clinger || npc.type == NPCID.IchorSticker || npc.type == NPCID.DesertGhoulCorruption || npc.type == NPCID.DesertGhoulCrimson) {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<PreventEvilBurn>(), DropChance));
        }
    }
}
