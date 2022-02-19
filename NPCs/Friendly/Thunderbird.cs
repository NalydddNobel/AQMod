using AQMod.Common;
using AQMod.NPCs.AIs;
using AQMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Friendly
{
    public class Thunderbird : AIBird
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 5;
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
            npc.catchItem = (short)ModContent.ItemType<Items.Materials.ThunderousPlume>();
            npc.rarity = 1;
        }

        public override void AI()
        {
            if (npc.ai[1] < 60f)
            {
                npc.ai[1]++;
                npc.noGravity = false;
                return;
            }
            for (int i = 0; i < 4; i++) // run AI a lot lol
            {
                if (i != 0)
                    npc.position += npc.velocity;
                base.AI();
            }
            BirdSounds();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.Y, 0, AQNPC.GreenSlimeColor);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.spawnTileY < 250)
            {
                float chance = (WorldDefeats.DownedStarite || NPC.downedMechBossAny ? 2f : 1f) * (spawnInfo.playerInTown ? 2f : 1f);
                return chance;
            }
            return 0f;
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            for (int j = tileY; j < tileY + 100; j++)
            {
                if (Main.tile[tileX, j].active() && (Main.tile[tileX, j].Solid() || Main.tile[tileX, j].SolidTop()))
                {
                    return NPC.NewNPC(tileX * 16 + 8, j * 16 - 8, npc.type);
                }
            }
            return base.SpawnNPC(tileX, tileY);
        }
    }
}