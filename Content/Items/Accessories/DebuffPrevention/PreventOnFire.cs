
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Items.Accessories.DebuffPrevention;

public class PreventOnFire : ModItem {
    public static readonly int DropChance = 100;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.CloneShopValues(ItemID.ArmorPolish);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.buffImmune[BuffID.OnFire] = true;
        player.buffImmune[BuffID.OnFire3] = true;
    }

    [Gen.AequusNPC_ModifyNPCLoot]
    internal static void AddLoot(NPC npc, NPCLoot loot) {
        if (npc.type == NPCID.FireImp || npc.ToBannerItem() == ItemID.HellArmoredBonesBanner) {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<PreventOnFire>(), DropChance));
        }
    }
}
