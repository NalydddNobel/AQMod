using AQMod.Common;
using AQMod.NPCs.AIs;
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
            float chance = SpawnCondition.Sky.Chance * 0.01f * (SpawnCondition.TownCritter.Chance * 5f + 1f);
            if (WorldDefeats.DownedStarite || NPC.downedMechBossAny)
                chance *= 5f;
            return chance;
        }
    }
}
