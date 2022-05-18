using Aequus.Content;
using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public class FlawlessNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool[] damagedPlayers;
        public bool preventNoHitCheck;

        public FlawlessNPC()
        {
            damagedPlayers = new bool[Main.maxPlayers];
        }

        private void ResetNoHit(int player)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                    Main.npc[i].GetGlobalNPC<FlawlessNPC>().damagedPlayers[player] = false;
            }
        }

        public override void ResetEffects(NPC npc)
        {
            if (!preventNoHitCheck)
            {
                var manager = ModContent.GetInstance<FlawlessSystem>();
                for (int i = 0; i < manager.DamagedPlayers.Count; i++)
                {
                    damagedPlayers[manager.DamagedPlayers[i]] = true;
                }
            }
        }

        public static bool HasBeenNoHit(NPC npc, int player)
        {
            return HasBeenNoHit(npc, npc.GetGlobalNPC<FlawlessNPC>(), player);
        }

        public static bool HasBeenNoHit(NPC npc, FlawlessNPC noHit, int player)
        {
            return npc.playerInteraction[player] && !noHit.damagedPlayers[player];
        }
    }
}