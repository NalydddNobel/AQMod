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
            public static NecroStats Four => new NecroStats(2f, 1250f);

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
                [NPCID.UmbrellaSlime] = NecroStats.One,
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
                [NPCID.Drippler] = NecroStats.One,
                [NPCID.FlyingFish] = NecroStats.One,
                [NPCID.Tumbleweed] = NecroStats.One,
                [NPCID.GoblinArcher] = NecroStats.One,
                [NPCID.GoblinPeon] = NecroStats.One,
                [NPCID.GoblinThief] = NecroStats.One,
                [NPCID.GoblinWarrior] = NecroStats.One,
                [NPCID.ServantofCthulhu] = NecroStats.One,

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
                [NPCID.TheGroom] = NecroStats.Two,
                [NPCID.TheBride] = NecroStats.Two,
                [NPCID.ZombieMerman] = NecroStats.Two,
                [NPCID.EyeballFlyingFish] = NecroStats.Two,
                [NPCID.MisterStabby] = NecroStats.Two,
                [NPCID.SnowBalla] = NecroStats.Two,
                [NPCID.SnowmanGangsta] = NecroStats.Two,
                [NPCID.Parrot] = NecroStats.Two,
                [NPCID.QueenSlimeMinionPink] = NecroStats.Two,
                [NPCID.QueenSlimeMinionPurple] = NecroStats.Two,
                [NPCID.QueenSlimeMinionBlue] = NecroStats.Two,
                [NPCID.TheHungryII] = NecroStats.Two,
                [NPCID.Probe] = NecroStats.Two,

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
                [NPCID.Crimslime] = NecroStats.Three,
                [NPCID.Slimeling] = NecroStats.Three, // TODO: support net IDs better?
                [NPCID.CrimsonAxe] = NecroStats.Three,
                [NPCID.CursedHammer] = NecroStats.Three,
                [NPCID.EnchantedSword] = NecroStats.Three,
                [NPCID.Derpling] = NecroStats.Three,
                [NPCID.DesertDjinn] = NecroStats.Three,
                [NPCID.DesertGhoul] = NecroStats.Three,
                [NPCID.DesertGhoulCorruption] = NecroStats.Three,
                [NPCID.DesertGhoulCrimson] = NecroStats.Three,
                [NPCID.DesertGhoulHallow] = NecroStats.Three,
                [NPCID.FungoFish] = NecroStats.Three,
                [NPCID.Gastropod] = NecroStats.Three,
                [NPCID.GiantBat] = NecroStats.Three,
                [NPCID.GiantFungiBulb] = NecroStats.Three,
                [NPCID.GreenJellyfish] = NecroStats.Three,
                [NPCID.Herpling] = NecroStats.Three,
                [NPCID.HoppinJack] = NecroStats.Three,
                [NPCID.IceElemental] = NecroStats.Three,
                [NPCID.IceMimic] = NecroStats.Three,
                [NPCID.Mimic] = NecroStats.Three,
                [NPCID.IceTortoise] = NecroStats.Three,
                [NPCID.IchorSticker] = NecroStats.Three,
                [NPCID.IcyMerman] = NecroStats.Three,
                [NPCID.IlluminantBat] = NecroStats.Three,
                [NPCID.IlluminantSlime] = NecroStats.Three,
                [NPCID.DesertLamiaDark] = NecroStats.Three,
                [NPCID.DesertLamiaLight] = NecroStats.Three,
                [NPCID.Lavabat] = NecroStats.Three,
                [NPCID.PigronCorruption] = NecroStats.Three,
                [NPCID.PigronCrimson] = NecroStats.Three,
                [NPCID.PigronHallow] = NecroStats.Three,
                [NPCID.Pixie] = NecroStats.Three,
                [NPCID.PossessedArmor] = NecroStats.Three,
                [NPCID.DesertScorpionWalk] = NecroStats.Three,
                [NPCID.DesertScorpionWall] = NecroStats.Three,
                [NPCID.ToxicSludge] = NecroStats.Three,
                [NPCID.Werewolf] = NecroStats.Three,
                [NPCID.Wolf] = NecroStats.Three,
                [NPCID.SkeletonArcher] = NecroStats.Three,
                [NPCID.WanderingEye] = NecroStats.Three,
                [NPCID.Clown] = NecroStats.Three,
                [NPCID.ChatteringTeethBomb] = NecroStats.Three,
                [NPCID.SandShark] = NecroStats.Three,
                [NPCID.SandsharkCorrupt] = NecroStats.Three,
                [NPCID.SandsharkCrimson] = NecroStats.Three,
                [NPCID.SandsharkHallow] = NecroStats.Three,
                [NPCID.BloodSquid] = NecroStats.Three,
                [NPCID.AngryNimbus] = NecroStats.Three,
                [NPCID.RainbowSlime] = NecroStats.Three,
                [NPCID.GoblinSummoner] = NecroStats.Three,
                [NPCID.PirateCorsair] = NecroStats.Three,
                [NPCID.PirateCrossbower] = NecroStats.Three,
                [NPCID.PirateDeadeye] = NecroStats.Three,
                [NPCID.PirateDeckhand] = NecroStats.Three,
                [NPCID.PirateGhost] = NecroStats.Three,
                [NPCID.MothronSpawn] = NecroStats.Three,
                [NPCID.MothronEgg] = NecroStats.Three,
                [NPCID.Butcher] = NecroStats.Three,
                [NPCID.CreatureFromTheDeep] = NecroStats.Three,
                [NPCID.Frankenstein] = NecroStats.Three,
                [NPCID.Fritz] = NecroStats.Three,
                [NPCID.Scarecrow1] = NecroStats.Three,
                [NPCID.Scarecrow2] = NecroStats.Three,
                [NPCID.Scarecrow3] = NecroStats.Three,
                [NPCID.Scarecrow4] = NecroStats.Three,
                [NPCID.Scarecrow5] = NecroStats.Three,
                [NPCID.Scarecrow6] = NecroStats.Three,
                [NPCID.Scarecrow7] = NecroStats.Three,
                [NPCID.Scarecrow8] = NecroStats.Three,
                [NPCID.Scarecrow9] = NecroStats.Three,
                [NPCID.Scarecrow10] = NecroStats.Three,
                [NPCID.Splinterling] = NecroStats.Three,
                [NPCID.Hellhound] = NecroStats.Three,
                [NPCID.Poltergeist] = NecroStats.Three,
                [NPCID.ZombieElf] = NecroStats.Three,
                [NPCID.ZombieElfBeard] = NecroStats.Three,
                [NPCID.ZombieElfGirl] = NecroStats.Three,
                [NPCID.GingerbreadMan] = NecroStats.Three,
                [NPCID.ElfArcher] = NecroStats.Three,
                [NPCID.Yeti] = NecroStats.Three,
                [NPCID.Nutcracker] = NecroStats.Three,
                [NPCID.NutcrackerSpinning] = NecroStats.Three,
                [NPCID.Flocko] = NecroStats.Three,
                [NPCID.ElfCopter] = NecroStats.Three,

                [NPCID.BlueArmoredBones] = NecroStats.Four,
                [NPCID.BlueArmoredBonesMace] = NecroStats.Four,
                [NPCID.BlueArmoredBonesNoPants] = NecroStats.Four,
                [NPCID.BlueArmoredBonesSword] = NecroStats.Four,
                [NPCID.BoneLee] = NecroStats.Four,
                [NPCID.BigMimicCorruption] = NecroStats.Four,
                [NPCID.BigMimicCrimson] = NecroStats.Four,
                [NPCID.BigMimicHallow] = NecroStats.Four,
                [NPCID.DiabolistRed] = NecroStats.Four,
                [NPCID.DiabolistWhite] = NecroStats.Four,
                [NPCID.FloatyGross] = NecroStats.Four,
                [NPCID.Wraith] = NecroStats.Four,
                [NPCID.FlyingSnake] = NecroStats.Four,
                [NPCID.GiantCursedSkull] = NecroStats.Four,
                [NPCID.GiantFlyingFox] = NecroStats.Four,
                [NPCID.GiantTortoise] = NecroStats.Four,
                [NPCID.HellArmoredBones] = NecroStats.Four,
                [NPCID.HellArmoredBonesMace] = NecroStats.Four,
                [NPCID.HellArmoredBonesSpikeShield] = NecroStats.Four,
                [NPCID.HellArmoredBonesSword] = NecroStats.Four,
                [NPCID.JungleCreeper] = NecroStats.Four,
                [NPCID.JungleCreeperWall] = NecroStats.Four,
                [NPCID.Lihzahrd] = NecroStats.Four,
                [NPCID.LihzahrdCrawler] = NecroStats.Four,
                [NPCID.MossHornet] = NecroStats.Four,
                [NPCID.Moth] = NecroStats.Four,
                [NPCID.Necromancer] = NecroStats.Four,
                [NPCID.NecromancerArmored] = NecroStats.Four,
                [NPCID.Paladin] = NecroStats.Four,
                [NPCID.RaggedCaster] = NecroStats.Four,
                [NPCID.RaggedCasterOpenCoat] = NecroStats.Four,
                [NPCID.RedDevil] = NecroStats.Four,
                [NPCID.RockGolem] = NecroStats.Four,
                [NPCID.RuneWizard] = NecroStats.Four,
                [NPCID.RustyArmoredBonesAxe] = NecroStats.Four,
                [NPCID.RustyArmoredBonesFlail] = NecroStats.Four,
                [NPCID.RustyArmoredBonesSword] = NecroStats.Four,
                [NPCID.RustyArmoredBonesSwordNoArmor] = NecroStats.Four,
                [NPCID.SkeletonCommando] = NecroStats.Four,
                [NPCID.SkeletonSniper] = NecroStats.Four,
                [NPCID.TacticalSkeleton] = NecroStats.Four,
                [NPCID.GoblinShark] = NecroStats.Four,
                [NPCID.BloodNautilus] = NecroStats.Four,
                [NPCID.IceGolem] = NecroStats.Four,
                [NPCID.SandElemental] = NecroStats.Four,
                [NPCID.Mothron] = NecroStats.Four,
                [NPCID.DeadlySphere] = NecroStats.Four,
                [NPCID.DrManFly] = NecroStats.Four,
                [NPCID.Eyezor] = NecroStats.Four,
                [NPCID.Nailhead] = NecroStats.Four,
                [NPCID.Psycho] = NecroStats.Four,
                [NPCID.Reaper] = NecroStats.Four,
                [NPCID.ThePossessed] = NecroStats.Four,
                [NPCID.Vampire] = NecroStats.Four,
                [NPCID.VampireBat] = NecroStats.Four,
                [NPCID.BrainScrambler] = NecroStats.Four,
                [NPCID.GigaZapper] = NecroStats.Four,
                [NPCID.GrayGrunt] = NecroStats.Four,
                [NPCID.MartianEngineer] = NecroStats.Four,
                [NPCID.MartianOfficer] = NecroStats.Four,
                [NPCID.MartianWalker] = NecroStats.Four,
                [NPCID.RayGunner] = NecroStats.Four,
                [NPCID.Scutlix] = NecroStats.Four,
                [NPCID.MartianTurret] = NecroStats.Four,
                [NPCID.MartianSaucerCore] = NecroStats.Four,
                [NPCID.MourningWood] = NecroStats.Four,
                [NPCID.HeadlessHorseman] = NecroStats.Four,
                [NPCID.Krampus] = NecroStats.Four,
                [NPCID.PresentMimic] = NecroStats.Four,
                [NPCID.Everscream] = NecroStats.Four,
                [NPCID.SantaNK1] = NecroStats.Four,
                [NPCID.IceQueen] = NecroStats.Four,
                [NPCID.NebulaBeast] = NecroStats.Four,
                [NPCID.NebulaBrain] = NecroStats.Four,
                [NPCID.NebulaSoldier] = NecroStats.Four,
                [NPCID.SolarCorite] = NecroStats.Four,
                [NPCID.SolarDrakomire] = NecroStats.Four,
                [NPCID.SolarSpearman] = NecroStats.Four,
                [NPCID.SolarSolenian] = NecroStats.Four,
                [NPCID.SolarSroller] = NecroStats.Four,
                [NPCID.StardustJellyfishBig] = NecroStats.Four,
                [NPCID.StardustSoldier] = NecroStats.Four,
                [NPCID.StardustSpiderBig] = NecroStats.Four,
                [NPCID.StardustSpiderSmall] = NecroStats.Four,
                [NPCID.StardustWormHead] = NecroStats.Four,
                [NPCID.VortexRifleman] = NecroStats.Four,
                [NPCID.VortexSoldier] = NecroStats.Four,
                [NPCID.AncientCultistSquidhead] = NecroStats.Four,
                [NPCID.BigMimicJungle] = NecroStats.Four,
                [NPCID.GoldenSlime] = NecroStats.Four,

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
                npc = IModCallable.UnboxIntoInt(args[1]);
                float tier = IModCallable.UnboxIntoFloat(args[2]);
                float viewDistance = args.Length == 4 ? IModCallable.UnboxIntoFloat(args[3]) : 800f;
                if (Aequus.LogMore)
                    Aequus.Instance.Logger.Info("Adding necromancy data for: " + Lang.GetNPCName(npc) + " -- Tier: " + tier + ", SightRange: " + viewDistance + " --");
                NPCs.AddOrAdjust(npc, new NecroStats(tier, viewDistance));
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