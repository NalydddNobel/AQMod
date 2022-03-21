using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public class LootLoopingHelper : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public bool isNPCLootLooping;
        public static byte Current = 0;

        public override void NPCLoot(NPC npc)
        {
            if (npc.SpawnedFromStatue || NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type])
                return;
            if (!isNPCLootLooping && !npc.boss && !npc.friendly && !AQNPC.Sets.Instance.NoSpoilLoot.Contains(npc.type))
            {
                byte p = Player.FindClosest(npc.position, npc.width, npc.height);
                var plr = Main.player[p];
                var aQPlayer = plr.GetModPlayer<AQPlayer>();
                if (aQPlayer.lootIterations > 0)
                {
                    if ((int)npc.extraValue > 0)
                    {
                        AQItem.DropMoney((int)npc.extraValue, npc.getRect());
                        npc.extraValue = 0;
                    }
                    isNPCLootLooping = true;
                    for (Current = 0; Current < aQPlayer.lootIterations; Current++)
                    {
                        npc.NPCLoot();
                    }
                    isNPCLootLooping = false;
                    Current = 0;
                }
            }
        }
    }
}