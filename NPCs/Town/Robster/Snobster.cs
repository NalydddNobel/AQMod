using AQMod.Common;
using AQMod.Common.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Town.Robster
{
    public class Snobster : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 24;
            item.consumable = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 10;
            item.useAnimation = 15;
            item.makeNPC = (short)ModContent.NPCType<SnobsterCritter>();
        }
    }

    public class SnobsterCritter : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 4;
            Main.npcCatchable[npc.type] = true;
        }

        public override void SetDefaults()
        {
            npc.width = 12;
            npc.height = 12;
            npc.aiStyle = -1;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 5;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.npcSlots = 0.5f;
            npc.catchItem = (short)ModContent.ItemType<Snobster>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.t_Slime, npc.velocity.X, npc.velocity.Y, 0, AQNPC.GreenSlime);
            }
        }

        public override void AI()
        {
            if (npc.collideY && npc.velocity.Y >= 0f)
            {
                npc.localAI[0] = 0f;
            }
            if (npc.collideX && npc.velocity.X.Abs() > 1f)
            {
                npc.velocity.X = -npc.oldVelocity.X * 0.9f;
            }
            if (npc.velocity.Y == 0)
            {
                npc.ai[0]++;
                npc.velocity.X *= 0.9f;
                if (npc.ai[0] > 50f)
                {
                    npc.ai[0] = Main.rand.Next(-10, 10);
                    npc.direction = Main.rand.NextBool() ? -1 : 1;
                    npc.spriteDirection = npc.direction;
                    npc.velocity.Y = -7f;
                    npc.velocity.X = 4f * npc.direction;
                    npc.localAI[0] = 1f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if ((int)npc.localAI[0] == 1)
            {
                if (npc.velocity.Y < -0.1f)
                {
                    npc.frame.Y = frameHeight * 2;
                }
                else if (npc.velocity.Y > 0.1f)
                {
                    npc.frame.Y = frameHeight * 3;
                }
            }
            else
            {
                npc.frameCounter++;
                if (npc.frameCounter < 6.0)
                {
                    npc.frame.Y = 0;
                }
                else
                {
                    if (npc.frameCounter >= 12.0)
                    {
                        npc.frameCounter = 0.0;
                    }
                    npc.frame.Y = frameHeight;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float chance = SpawnCondition.Ocean.Chance * 0.1f * (SpawnCondition.TownCritter.Chance * 5f + 1f);
            if (WorldDefeats.DownedCrabson)
            {
                chance *= 5f;
            }
            return chance;
        }
    }
}