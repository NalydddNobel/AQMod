using Aequu2.Old.Content.Items.Accessories.OnHitDebuffs;
using Aequu2.Old.Content.Items.Materials.Energies;
using Terraria.GameContent.ItemDropRules;

namespace Aequu2.Old.Common;

public class EnemyDropEdits : GlobalNPC {
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        switch (npc.type) {
            case NPCID.DarkCaster: {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BoneRing>(), chanceDenominator: 30));
                }
                break;

            case NPCID.WallofFlesh: {
                    npcLoot.Add(new DropPerPlayerOnThePlayer(EnergyMaterial.Demonic.Type, 1, 3, 3, null));
                }
                break;

            case NPCID.Plantera: {
                    npcLoot.Add(new DropPerPlayerOnThePlayer(EnergyMaterial.Organic.Type, 1, 3, 3, null));
                }
                break;
        }
    }
}

//public class ItemDropEdits : GlobalItem {
//    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
//    }
//}
