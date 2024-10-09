
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Items.Accessories.DebuffPrevention;

public class PreventAcidVenom : ModItem {
    public static readonly int DropChance = 100;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.CloneShopValues(ItemID.ArmorPolish);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.buffImmune[BuffID.Venom] = true;
        player.buffImmune[BuffID.Webbed] = true;
    }

    [Gen.AequusNPC_ModifyNPCLoot]
    internal static void AddLoot(NPC npc, NPCLoot loot) {
        if (npc.ToBannerItem() == ItemID.BlackRecluseBanner || npc.ToBannerItem() == ItemID.RavagerScorpionBanner || npc.ToBannerItem() == ItemID.JungleCreeperBanner) {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<PreventAcidVenom>(), DropChance));
        }
    }
}
