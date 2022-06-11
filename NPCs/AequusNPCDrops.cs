using Aequus.Items.Consumables.Foods;
using Aequus.Items.Weapons.Melee;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public class AequusNPCDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.UndeadViking || npc.type == NPCID.ArmoredViking)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystalDagger>(), 12));
            }
            else if (npc.type == NPCID.DevourerHead || npc.type == NPCID.GiantWormHead || npc.type == NPCID.BoneSerpentHead || npc.type == NPCID.TombCrawlerHead
                || npc.type == NPCID.DiggerHead || npc.type == NPCID.DuneSplicerHead || npc.type == NPCID.SeekerHead || npc.type == NPCID.BloodEelHead)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 25));
            }
        }
    }
}