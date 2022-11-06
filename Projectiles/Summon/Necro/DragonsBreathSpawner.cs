using Aequus.Content.Necromancy;
using Terraria;
using Terraria.ID;

namespace Aequus.Projectiles.Summon.Necro
{
    public class DragonsBreathSpawner : GhostSpawner
    {
        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var npc = NPC.NewNPCDirect(Projectile.GetSource_Death("Aequus:NecromancySpawn"), Projectile.Center, NPCID.CultistDragonHead);
                if (npc.whoAmI == 0)
                {
                    Kill(timeLeft);
                    npc.active = false;
                    PacketSystem.SyncNPC(npc);
                    return;
                }
                npc.ai[3] = npc.whoAmI;
                npc.realLife = npc.whoAmI;
                OnSpawnZombie(npc, npc.GetGlobalNPC<NecromancyNPC>());
                npc.netUpdate = true;
                int lastSegment = npc.whoAmI;
                for (int i = 0; i < 7; i++)
                {
                    int npcID = NPCID.CultistDragonHead + i + 1;
                    if (i > 1)
                    {
                        npcID -= 2;
                    }
                    int currentSegment = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), npcID, npc.whoAmI);
                    Main.npc[currentSegment].ai[3] = npc.whoAmI;
                    Main.npc[currentSegment].realLife = npc.whoAmI;
                    Main.npc[currentSegment].ai[1] = lastSegment;
                    Main.npc[currentSegment].CopyInteractions(npc);
                    Main.npc[lastSegment].ai[0] = currentSegment;
                    Main.npc[currentSegment].GetGlobalNPC<NecromancyNPC>().slotsConsumed = 0;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, currentSegment);
                    lastSegment = currentSegment;
                }
            }
        }
    }
}