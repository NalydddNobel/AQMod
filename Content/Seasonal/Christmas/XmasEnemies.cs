using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.Seasonal.Christmas
{
    public class XmasEnemies : GlobalNPC
    {
        public bool xmasEnemySwapCheck;

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public override void AI(NPC npc)
        {
            if (XmasSeeds.XmasWorld)
            {
                xmasEnemySwapCheck = true;
                if (AQNPC.Sets.IsASkeleton(npc.type, includeHardmodeSkeletons: false, includeHoplites: true))
                {
                    npc.SetDefaults(NPCID.UndeadViking);
                }
                if (AQNPC.Sets.IsAZombie(npc.type, includeBloodZombies: true))
                {
                    npc.SetDefaults(NPCID.SnowFlinx);
                }
                if (npc.type == NPCID.Bunny)
                {
                    npc.SetDefaults(NPCID.BunnyXmas);
                }
                if (npc.type != NPCID.IceSlime && npc.type != NPCID.SpikedIceSlime)
                {
                    if (npc.type == NPCID.SlimeSpiked || 
                        npc.type == NPCID.SpikedJungleSlime || 
                        npc.type == ModContent.NPCType<NPCs.Monsters.GaleStreams.WhiteSlime>())
                    {
                        npc.SetDefaults(NPCID.SpikedIceSlime);
                    }
                    else if (npc.aiStyle == 1)
                    {
                        npc.SetDefaults(NPCID.IceSlime);
                    }
                }
                if (npc.type == NPCID.FireImp)
                {
                    npc.SetDefaults(NPCID.DarkCaster);
                }
                if (npc.type == NPCID.ArmoredSkeleton)
                {
                    npc.SetDefaults(NPCID.MisterStabby);
                }
                if (npc.type == NPCID.SkeletonArcher)
                {
                    npc.SetDefaults(Main.rand.NextBool() ? NPCID.SnowBalla : NPCID.SnowmanGangsta);
                }
                if (npc.type == NPCID.CochinealBeetle || npc.type == NPCID.LacBeetle)
                {
                    npc.SetDefaults(NPCID.CyanBeetle);
                }
            }
        }

        public override void NPCLoot(NPC npc)
        {
            if (npc.type == NPCID.Retinazer || 
                npc.type == NPCID.Spazmatism || 
                npc.type == NPCID.SkeletronPrime || 
                npc.type == NPCID.TheDestroyer)
            {
                bool dropTempleKey = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
                if (dropTempleKey)
                {
                    if (npc.type == NPCID.Retinazer)
                    {
                        dropTempleKey = NPC.AnyNPCs(NPCID.Spazmatism);
                    }
                    else if (npc.type == NPCID.Spazmatism)
                    {
                        dropTempleKey = NPC.AnyNPCs(NPCID.Retinazer);
                    }
                }
                if (dropTempleKey)
                {
                    Item.NewItem(npc.getRect(), ItemID.TempleKey);
                }
            }
        }
    }
}