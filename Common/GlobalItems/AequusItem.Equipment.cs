using Aequus.NPCs.Monsters.Event.GaleStreams;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items {
    public partial class AequusItem {
        public override void UpdateEquip(Item item, Player player) {
            if (!AequusPlayer.EquipmentModifierUpdate) {
                equipEmpowerment = null;
            }
            CheckNameTag(item);
            UpdateEquip_Prefixes(item, player);
            if (defenseChange < 0) {
                player.Aequus().negativeDefense -= defenseChange;
            }
            else {
                player.statDefense += defenseChange;
            }
            if (item.type == ItemID.WormScarf) {
                player.Aequus().accWormScarf = item;
            }
            else if (item.type == ItemID.BoneHelm) {
                player.Aequus().accBoneHelm = item;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual) {
            CheckNameTag(item);
            UpdateAccessory_Prefixes(item, player, hideVisual);
            if (defenseChange < 0) {
                player.Aequus().negativeDefense -= defenseChange;
            }
            if (item.type == ItemID.RoyalGel || player.npcTypeNoAggro[NPCID.BlueSlime]) {
                player.npcTypeNoAggro[ModContent.NPCType<WhiteSlime>()] = true;
            }
        }
    }
}