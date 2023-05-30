using Aequus.Content;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.NPCs {
    public class FlawlessGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool[] damagedPlayers;
        public bool preventNoHitCheck;

        public FlawlessGlobalNPC()
        {
            damagedPlayers = new bool[Main.maxPlayers];
        }

        public override void ResetEffects(NPC npc)
        {
            if (!preventNoHitCheck)
            {
                foreach (byte p in FlawlessFightSystem.DamagedPlayers)
                {
                    damagedPlayers[p] = true;
                }
            }
        }

        public void ResetNoHit(int player)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                    Main.npc[i].GetGlobalNPC<FlawlessGlobalNPC>().damagedPlayers[player] = false;
            }
        }

        public static bool HasBeenNoHit(NPC npc, int player)
        {
            return HasBeenNoHit(npc, npc.GetGlobalNPC<FlawlessGlobalNPC>(), player);
        }

        public static bool HasBeenNoHit(NPC npc, FlawlessGlobalNPC noHit, int player)
        {
            return npc.playerInteraction[player] && !noHit.damagedPlayers[player];
        }
    }
}
