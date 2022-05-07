using Aequus.Items.Weapons.Summon.Necro;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Catalogues
{
    public sealed class NecromancyTypes : LoadableType, IModCallable
    {
        public struct NecroStats
        {
            /// <summary>
            /// For enemies which cannot be turned into player zombies
            /// </summary>
            public static NecroStats None => new NecroStats(0f);
            public static NecroStats One => new NecroStats(1f, 800f);
            public static NecroStats Two => new NecroStats(2f, 800f);
            public static NecroStats Three => new NecroStats(2f, 1000f);

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
        public static Dictionary<int, NecroStats> NPCs { get; private set; }

        public override void Load()
        {
            NecromancyDebuffs = new List<int>();

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
                [NPCID.LarvaeAntlion] = NecroStats.One,
                [NPCID.GreekSkeleton] = NecroStats.One,
                [NPCID.FaceMonster] = NecroStats.One,
                [NPCID.JungleBat] = NecroStats.One,
                [NPCID.UndeadViking] = NecroStats.One,
                [NPCID.WallCreeper] = NecroStats.One,
                [NPCID.WallCreeperWall] = NecroStats.One,
                [NPCID.BloodCrawler] = NecroStats.One,
                [NPCID.BloodCrawlerWall] = NecroStats.One,

                [NPCID.Crimera] = NecroStats.Two,
                [NPCID.EaterofSouls] = NecroStats.Two,
                [NPCID.AngryBones] = NecroStats.Two,
                [NPCID.GiantFlyingAntlion] = NecroStats.Two,
                [NPCID.GiantWalkingAntlion] = NecroStats.Two,
                [NPCID.CursedSkull] = NecroStats.Two,
                [NPCID.Demon] = NecroStats.Two,
                [NPCID.DungeonSlime] = NecroStats.Two,
                [NPCID.GiantShelly] = NecroStats.Two,
                [NPCID.GiantShelly2] = NecroStats.Two,
                [NPCID.GraniteFlyer] = NecroStats.Two,
                [NPCID.Harpy] = NecroStats.Two,
                [NPCID.Hellbat] = NecroStats.Two,
                [NPCID.MeteorHead] = NecroStats.Two,
                [NPCID.Shark] = NecroStats.Two,
                [NPCID.SpikedIceSlime] = NecroStats.Two,
                [NPCID.SpikedJungleSlime] = NecroStats.Two,
                [NPCID.UndeadMiner] = NecroStats.Two,
                [NPCID.VoodooDemon] = NecroStats.Two,

                [NPCID.AnomuraFungus] = NecroStats.Three,
                [NPCID.DoctorBones] = NecroStats.Three,
                [NPCID.FungiBulb] = NecroStats.Three,
                [NPCID.Ghost] = NecroStats.Three,
                [NPCID.MushiLadybug] = NecroStats.Three,
                [NPCID.Nymph] = NecroStats.Three,
                [NPCID.ZombieMushroom] = NecroStats.Three,
                [NPCID.ZombieMushroomHat] = NecroStats.Three,
                [NPCID.AnglerFish] = NecroStats.Three,
                [NPCID.Arapaima] = NecroStats.Three,
                [NPCID.ArmoredSkeleton] = NecroStats.Three,
                [NPCID.ArmoredViking] = NecroStats.Three,
                [NPCID.DesertBeast] = NecroStats.Three,
                [NPCID.BlackRecluse] = NecroStats.Three,
                [NPCID.BlackRecluseWall] = NecroStats.Three,
                [NPCID.BloodFeeder] = NecroStats.Three,
                [NPCID.BloodJelly] = NecroStats.Three,
                [NPCID.Mummy] = NecroStats.Three,
                [NPCID.BloodMummy] = NecroStats.Three,
                [NPCID.DarkMummy] = NecroStats.Three,
                [NPCID.LightMummy] = NecroStats.Three,
                [NPCID.ChaosElemental] = NecroStats.Three,
                [NPCID.CorruptSlime] = NecroStats.Three,
                [NPCID.Corruptor] = NecroStats.Three,

                [NPCID.Gnome] = NecroStats.None,
                [NPCID.MotherSlime] = NecroStats.None,
                [NPCID.GiantWormHead] = NecroStats.None,
                [NPCID.GiantWormBody] = NecroStats.None,
                [NPCID.GiantWormTail] = NecroStats.None,
                [NPCID.BoneSerpentHead] = NecroStats.None,
                [NPCID.BoneSerpentBody] = NecroStats.None,
                [NPCID.BoneSerpentTail] = NecroStats.None,
                [NPCID.DevourerHead] = NecroStats.None,
                [NPCID.DevourerBody] = NecroStats.None,
                [NPCID.DevourerTail] = NecroStats.None,
                [NPCID.TombCrawlerHead] = NecroStats.None,
                [NPCID.TombCrawlerBody] = NecroStats.None,
                [NPCID.TombCrawlerTail] = NecroStats.None,
                [NPCID.DungeonGuardian] = NecroStats.None,
                [NPCID.DarkCaster] = NecroStats.None,
                [NPCID.FireImp] = NecroStats.None,
                [NPCID.ManEater] = NecroStats.None,
                [NPCID.Snatcher] = NecroStats.None,
                [NPCID.Tim] = NecroStats.None,
                [NPCID.AngryTrapper] = NecroStats.None,
            };

            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.Zombie, vanillaOnly: true))
            {
                NPCs[i] = NecroStats.One;
            }
            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.DemonEye, vanillaOnly: true))
            {
                NPCs[i] = NecroStats.One;
            }
            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.Hornet, vanillaOnly: true))
            {
                NPCs[i] = NecroStats.Two;
            }
        }

        /// <summary>
        /// Adds a NecroStats data for an npc index in <see cref="NPCs"/>
        /// <para>Parameter 1: NPC Type (short)</para>
        /// <para>Parameter 2: Tier (float), <see cref="ZombieScepter"/> is tier 1, <see cref="InsurgencyScepter"/> is tier 4</para>
        /// <para>Parameter 3 (Optional): View range (float), how close a slave needs to be to an enemy in order for it to target it. Defaults to 800</para>
        /// <para>A successful mod call would look like:</para>
        /// <code>aequus.Call("NecroStats", ModContent.NPCType{...}(), 1f);</code> OR
        /// <code>aequus.Call("NecroStats", ModContent.NPCType{...}(), 1f, 800f);</code>
        /// <para>Please handle these mod calls in <see cref="Mod.PostSetupContent"/>. As buff immunities are setup in <see cref="Aequus.AddRecipes"/></para>
        /// </summary>
        /// <param name="aequus"></param>
        /// <param name="args"></param>
        /// <returns>'Success' when correctly handled. 'Failure' when improperly handled</returns>
        public object HandleModCall(Aequus aequus, object[] args)
        {
            int npc = 0;
            try
            {
                npc = (short)args[1];
                float tier = (float)args[2];
                float viewDistance = args.Length == 4 ? (float)args[3] : 800f;

            }
            catch
            {
                string name = "Unknown";
                if (npc > NPCID.NegativeIDCount && npc < NPCLoader.NPCCount)
                {
                    name = Lang.GetNPCName(npc).Value;
                }
                Aequus.Instance.Logger.Error("Failed handling a mod call for NecroStats. {NPC Type: " + npc + ", Potential Name: " + name + "}");
                return IModCallable.Failure;
            }

            return IModCallable.Success;
        }
    }
}