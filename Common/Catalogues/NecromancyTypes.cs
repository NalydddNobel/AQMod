using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria.ID;

namespace Aequus.Common.Catalogues
{
    public sealed class NecromancyTypes : LoadableType
    {
        public struct NecroStats
        {
            /// <summary>
            /// For enemies which cannot be turned into player zombies
            /// </summary>
            public static NecroStats None => new NecroStats(0f);
            public static NecroStats One => new NecroStats(1f, 800f);
            public static NecroStats Two => new NecroStats(2f, 800f);

            public float PowerNeeded;
            public float ViewDistance;

            public NecroStats(float power, float view = 800f)
            {
                PowerNeeded = power;
                ViewDistance = view;
            }

            public override bool Equals([NotNullWhen(true)] object obj)
            {
                return obj is NecroStats nec && nec.PowerNeeded == PowerNeeded;
            }

            public override int GetHashCode()
            {
                return new { PowerNeeded, }.GetHashCode();
            }

            public static bool operator ==(NecroStats value, NecroStats value2)
            {
                return value.Equals(value2);
            }
            public static bool operator !=(NecroStats value, NecroStats value2)
            {
                return !value.Equals(value2);
            }
        }

        public static List<int> NecromancyDebuffs { get; private set; }
        public static Dictionary<int, float> StaffTiers { get; private set; }
        public static Dictionary<int, NecroStats> NPCs { get; private set; }

        public override void Load()
        {
            NecromancyDebuffs = new List<int>();
            StaffTiers = new Dictionary<int, float>();

            NPCs = new Dictionary<int, NecroStats>()
            {
                [NPCID.BlueSlime] = NecroStats.One,
                [NPCID.ZombieRaincoat] = NecroStats.One,
                [NPCID.ZombieEskimo] = NecroStats.One,
                [NPCID.MaggotZombie] = NecroStats.One,
                [NPCID.BloodZombie] = NecroStats.One,
                [NPCID.CaveBat] = NecroStats.One,
                [NPCID.SporeBat] = NecroStats.One,
                [NPCID.Bee] = NecroStats.One,
                [NPCID.BeeSmall] = NecroStats.One,
                [NPCID.BlueJellyfish] = NecroStats.One,
                [NPCID.PinkJellyfish] = NecroStats.One,
                [NPCID.Crab] = NecroStats.One,
                [NPCID.IceSlime] = NecroStats.One,
                [NPCID.LacBeetle] = NecroStats.One,
                [NPCID.CochinealBeetle] = NecroStats.One,
                [NPCID.CyanBeetle] = NecroStats.One,
                [NPCID.Piranha] = NecroStats.One,
                [NPCID.Vulture] = NecroStats.One,
                [NPCID.Raven] = NecroStats.One,
                [NPCID.SnowFlinx] = NecroStats.One,
                [NPCID.Antlion] = NecroStats.One,
                [NPCID.Squid] = NecroStats.One,
                [NPCID.Skeleton] = NecroStats.One,
                [NPCID.SkeletonAlien] = NecroStats.One,
                [NPCID.SkeletonAstonaut] = NecroStats.One,
                [NPCID.SkeletonTopHat] = NecroStats.One,
                [NPCID.HeadacheSkeleton] = NecroStats.One,
                [NPCID.MisassembledSkeleton] = NecroStats.One,
                [NPCID.PantlessSkeleton] = NecroStats.One,
                [NPCID.SporeSkeleton] = NecroStats.One,
                [NPCID.SlimeSpiked] = NecroStats.One,
                [NPCID.Crawdad] = NecroStats.One,
                [NPCID.Crawdad2] = NecroStats.One,
                [NPCID.Salamander] = NecroStats.One,
                [NPCID.Salamander2] = NecroStats.One,
                [NPCID.Salamander3] = NecroStats.One,
                [NPCID.Salamander4] = NecroStats.One,
                [NPCID.Salamander5] = NecroStats.One,
                [NPCID.Salamander6] = NecroStats.One,
                [NPCID.Salamander7] = NecroStats.One,
                [NPCID.Salamander8] = NecroStats.One,
                [NPCID.Salamander9] = NecroStats.One,
                [NPCID.IceBat] = NecroStats.One,
                [NPCID.SeaSnail] = NecroStats.One,
                [NPCID.GoblinScout] = NecroStats.One,
                [NPCID.GraniteGolem] = NecroStats.One,
                [NPCID.FaceMonster] = NecroStats.One,
                [NPCID.FlyingAntlion] = NecroStats.One,
                [NPCID.WalkingAntlion] = NecroStats.One,
                [NPCID.GreekSkeleton] = NecroStats.One,

                [NPCID.Gnome] = NecroStats.None,
                [NPCID.WallCreeper] = NecroStats.None,
                [NPCID.WallCreeperWall] = NecroStats.None,
                [NPCID.BloodCrawler] = NecroStats.None,
                [NPCID.BloodCrawlerWall] = NecroStats.None,
                [NPCID.MotherSlime] = NecroStats.None,
                [NPCID.GiantWormHead] = NecroStats.None,
                [NPCID.GiantWormBody] = NecroStats.None,
                [NPCID.GiantWormTail] = NecroStats.None,
                [NPCID.DungeonGuardian] = NecroStats.None,
            };

            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.Zombie, vanillaOnly: true))
            {
                NPCs[i] = NecroStats.One;
            }
            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.DemonEye, vanillaOnly: true))
            {
                NPCs[i] = NecroStats.One;
            }
        }
    }
}