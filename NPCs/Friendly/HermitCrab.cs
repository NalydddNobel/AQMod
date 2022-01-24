using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Friendly
{
    public class HermitCrab : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 24;
            npc.height = 24;
            npc.lifeMax = 100;
            npc.knockBackResist = 0.002f;
            npc.aiStyle = -1;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            npc.friendly = true;
            npc.gfxOffY = -4;
            npc.behindTiles = true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 1.25f);
                }
                var center = npc.Center;
                var offset = new Vector2(6f * npc.direction, 0f);
                int type = mod.GetGoreSlot("Gores/HermitCrab_2");
                Gore.NewGore(center + offset, npc.velocity, type);
                Gore.NewGore(center + offset + new Vector2(2f * npc.direction, 0f), npc.velocity, type);
                Gore.NewGore(center + new Vector2(-8f * npc.direction, 0f), npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_3"));
                Gore.NewGore(center, npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_0"));
                Gore.NewGore(center, npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_4"));
                switch ((int)npc.localAI[0])
                {
                    default:
                        Gore.NewGore(center, npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_1"));
                        break;
                }
            }
            else
            {
                for (int i = 0; i < damage / 5; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 0.9f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<PlayerBiomes>().zoneCrabCrevice)
                return 0.01f;
            return 0f;
        }
    }
}