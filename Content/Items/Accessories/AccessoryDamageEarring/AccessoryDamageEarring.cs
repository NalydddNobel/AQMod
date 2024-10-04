using Aequus.Common.Utilities;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.AccessoryDamageEarring;

[Gen.AequusPlayer_ResetField<StatModifier>("accessoryDamage")]
public class AccessoryDamageEarring : ModItem {
    public static readonly float AccessoryDamage = 0.75f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ALanguage.Percent(AccessoryDamage));

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.CloneShopValues(ItemID.StarCloak);
    }

    public override void UpdateEquip(Player player) {
        player.GetModPlayer<AequusPlayer>().accessoryDamage += AccessoryDamage;
    }

    [Gen.AequusNPC_ModifyNPCLoot]
    public static void AddNPCLoot(NPC npc, AequusNPC aequus, NPCLoot loot) {
        if (npc.type == NPCID.IlluminantBat || npc.type == NPCID.IlluminantSlime) {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<AccessoryDamageEarring>(), 60));
        }
    }
}
