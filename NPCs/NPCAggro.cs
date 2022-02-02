using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public sealed class NPCAggro : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public bool isAggro = false;
        public bool manuallyChecksAggro = false;

        public override void PostAI(NPC npc)
        {
            if (isAggro || manuallyChecksAggro)
                return;
            if (npc.life < npc.lifeMax || npc.boss)
            {
                isAggro = true;
                return;
            }
            if (npc.dontTakeDamage || npc.friendly || npc.townNPC || npc.lifeMax < 5)
            {
                isAggro = false;
                return;
            }
            if (npc.aiStyle == AQNPC.AIStyles.Slimes)
            {
                isAggro = DetermineAggro_AI_001_Slimes(npc);
            }
            else if (npc.aiStyle == AQNPC.AIStyles.Fighters)
            {
                isAggro = DetermineAggro_AI_003_Fighters(npc);
            }
            else if (npc.aiStyle == AQNPC.AIStyles.Mimics)
            {
                isAggro = DetermineAggro_AI_025_Mimics(npc);
            }
            else
            {
                int p = Player.FindClosest(npc.position, npc.width, npc.height);
                if (Main.player[p].active && !Main.player[p].dead)
                {
                    if (npc.Distance(Main.player[p].Center) < 200f)
                    {
                        isAggro = true;
                        return;
                    }
                    if (npc.velocity != Vector2.Zero && npc.velocity.RotatedBy((Main.player[p].Center - npc.Center).ToRotation()).ToRotation().Abs() < 0f)
                    {
                        isAggro = true;
                    }
                }
            }
        }

        public static bool DetermineAggro_AI_001_Slimes(NPC npc)
        {
            return !Main.dayTime || npc.life != npc.lifeMax || npc.position.Y > Main.worldSurface * 16.0 || Main.slimeRain
                ? true
                : npc.type == NPCID.CorruptSlime || npc.type == NPCID.Crimslime || npc.type == NPCID.HoppinJack || npc.type == NPCID.RainbowSlime;
        }

        public static bool DetermineAggro_AI_003_Fighters(NPC npc)
        {
            int num119 = 60;
            if (npc.type == NPCID.ChaosElemental)
            {
                num119 = 180;
            }
            return npc.ai[3] < num119 &&
            (Main.eclipse || !Main.dayTime || npc.position.Y > Main.worldSurface * 16f ||
            (Main.invasionType == 1 && (npc.type == NPCID.Yeti || npc.type == NPCID.ElfArcher)) ||
            (Main.invasionType == 1 && (npc.type == NPCID.GoblinPeon || npc.type == NPCID.GoblinThief || npc.type == NPCID.GoblinWarrior || npc.type == NPCID.GoblinArcher || npc.type == NPCID.GoblinSummoner)) ||
            npc.type == NPCID.GoblinScout || (Main.invasionType == 3 && npc.type >= NPCID.PirateDeckhand && npc.type <= NPCID.PirateCaptain) ||
            (Main.invasionType == 4 && (npc.type == NPCID.BrainScrambler || npc.type == NPCID.RayGunner || npc.type == NPCID.MartianOfficer || npc.type == NPCID.GrayGrunt || npc.type == NPCID.MartianEngineer || npc.type == NPCID.GigaZapper || npc.type == NPCID.Scutlix || npc.type == NPCID.MartianWalker))
            || npc.type == NPCID.AngryBones || npc.type == NPCID.AngryBonesBig || npc.type == NPCID.AngryBonesBigMuscle || npc.type == NPCID.AngryBonesBigHelmet
            || npc.type == NPCID.CorruptBunny || npc.type == NPCID.Crab || npc.type == NPCID.ArmoredSkeleton || npc.type == NPCID.Mummy || npc.type == NPCID.DarkMummy || npc.type == NPCID.LightMummy
            || npc.type == NPCID.SkeletonArcher || npc.type == NPCID.ChaosElemental || npc.type == NPCID.CorruptPenguin || npc.type == NPCID.FaceMonster || npc.type == NPCID.SnowFlinx
            || npc.type == NPCID.Lihzahrd || npc.type == NPCID.LihzahrdCrawler || npc.type == NPCID.IcyMerman || npc.type == NPCID.CochinealBeetle
            || npc.type == NPCID.CyanBeetle || npc.type == NPCID.LacBeetle || npc.type == NPCID.SeaSnail || npc.type == NPCID.BloodCrawler || npc.type == NPCID.IceGolem
            || npc.type == NPCID.ZombieMushroom || npc.type == NPCID.ZombieMushroomHat || npc.type == NPCID.AnomuraFungus || npc.type == NPCID.MushiLadybug ||
            npc.type == NPCID.SkeletonSniper || npc.type == NPCID.TacticalSkeleton || npc.type == NPCID.SkeletonCommando || npc.type == NPCID.CultistArcherBlue || npc.type == NPCID.CultistArcherWhite || npc.type == NPCID.CrimsonBunny
            || npc.type == NPCID.CrimsonPenguin || npc.type == NPCID.NebulaSoldier || (npc.type == NPCID.StardustSoldier && (npc.ai[1] >= 180f || npc.ai[1] < 90f))
            || npc.type == NPCID.StardustSpiderBig || npc.type == NPCID.VortexRifleman || npc.type == NPCID.VortexSoldier || npc.type == NPCID.VortexHornet || npc.type == NPCID.VortexLarva
            || npc.type == NPCID.WalkingAntlion || npc.type == NPCID.SolarDrakomire || npc.type == NPCID.SolarSolenian || (npc.type >= NPCID.DesertGhoul && npc.type <= NPCID.DesertGhoulHallow)
            || npc.type == NPCID.DesertLamiaLight || npc.type == NPCID.DesertLamiaDark || npc.type == NPCID.DesertScorpionWalk || npc.type == NPCID.DesertBeast);
        }

        public static bool DetermineAggro_AI_025_Mimics(NPC npc)
        {
            return (int)npc.ai[0] == 0;
        }

        private bool DetermineAggro_(NPC npc)
        {
            return false;
        }
    }
}