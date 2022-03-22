using AQMod.Common.Graphics;
using AQMod.Common.ID;
using AQMod.Content.World;
using AQMod.Dusts;
using AQMod.Effects;
using AQMod.Effects.Particles;
using AQMod.Items.Misc.Cursor;
using AQMod.Items.Potions;
using AQMod.NPCs;
using AQMod.NPCs.Bosses;
using AQMod.NPCs.Friendly;
using AQMod.NPCs.Monsters;
using AQMod.NPCs.Monsters.DemonSiegeMonsters;
using AQMod.NPCs.Monsters.GaleStreamsMonsters;
using AQMod.Projectiles;
using AQMod.Projectiles.Summon.Equips;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQNPC : GlobalNPC
    {
        public sealed class Sets
        {
            public static Sets Instance;

            public HashSet<int> NoSpoilLoot { get; private set; }
            public HashSet<int> HecktoplasmSpawn { get; private set; }
            public HashSet<int> WindUpdates { get; private set; }
            public HashSet<int> BossRelatedEnemy { get; private set; }
            public HashSet<int> IsACaveSkeleton { get; private set; }
            public HashSet<int> IsAZombie { get; private set; }
            public HashSet<int> DealsLessDamageToCata { get; private set; }
            public HashSet<int> CannotBeMeathooked { get; private set; }
            public HashSet<int> NoGlobalDrops { get; private set; }
            public HashSet<int> NoMapBlip { get; private set; }
            public HashSet<int> HotDamage { get; private set; }
            public HashSet<int> Corruption { get; private set; }
            public HashSet<int> Crimson { get; private set; }
            public HashSet<int> Unholy { get; private set; }
            public HashSet<int> Hallowed { get; private set; }

            public static bool TryAddTo(Mod mod, string name, HashSet<int> set)
            {
                int type = mod.NPCType(name);
                if (type != 0)
                {
                    return set.Add(type);
                }
                return false;
            }

            public static bool TryAddTo(Mod mod, string name, params HashSet<int>[] sets)
            {
                int type = mod.NPCType(name);
                if (type != 0)
                {
                    foreach (var set in sets)
                    {
                        set.Add(type);
                    }
                    return true;
                }
                return false;
            }

            public Sets()
            {
                SetUtils.Length = NPCLoader.NPCCount;
                SetUtils.GetIDFromType = (m, n) => m.NPCType(n);

                BossRelatedEnemy = new HashSet<int>()
                {
                    NPCID.ServantofCthulhu,
                    NPCID.PirateShipCannon,
                    NPCID.MartianSaucerTurret,
                    NPCID.MartianSaucerCore,
                    NPCID.MartianSaucerCannon,
                    NPCID.EaterofWorldsHead,
                    NPCID.EaterofWorldsBody,
                    NPCID.EaterofWorldsTail,
                    NPCID.TheDestroyerBody,
                    NPCID.TheDestroyerTail,
                    NPCID.GolemFistLeft,
                    NPCID.GolemFistRight,
                    NPCID.GolemHead,
                    NPCID.PrimeVice,
                    NPCID.PrimeLaser,
                    NPCID.PrimeSaw,
                    NPCID.PrimeCannon,
                    NPCID.MourningWood,
                    NPCID.Pumpking,
                    NPCID.PumpkingBlade,
                    NPCID.Everscream,
                    NPCID.SantaNK1,
                    NPCID.IceQueen,
                    NPCID.DD2DarkMageT1,
                    NPCID.DD2DarkMageT3,
                    NPCID.DD2OgreT2,
                    NPCID.DD2OgreT3,
                    NPCID.DD2Betsy,
                    NPCID.Creeper,
                    NPCID.BeeSmall,
                    NPCID.Bee,
                };

                IsACaveSkeleton = new HashSet<int>()
                {
                    NPCID.GreekSkeleton,
                    NPCID.Skeleton,
                    NPCID.SkeletonAlien,
                    NPCID.SkeletonAstonaut,
                    NPCID.SkeletonTopHat,
                    NPCID.HeadacheSkeleton,
                    NPCID.MisassembledSkeleton,
                    NPCID.PantlessSkeleton,
                    NPCID.BoneThrowingSkeleton,
                    NPCID.BoneThrowingSkeleton2,
                    NPCID.BoneThrowingSkeleton3,
                    NPCID.BoneThrowingSkeleton4,
                    NPCID.ArmoredSkeleton,
                    NPCID.SkeletonArcher,
                };

                IsAZombie = new HashSet<int>()
                {
                    NPCID.Zombie,
                    NPCID.BaldZombie,
                    NPCID.PincushionZombie,
                    NPCID.SlimedZombie,
                    NPCID.SwampZombie,
                    NPCID.TwiggyZombie,
                    NPCID.FemaleZombie,
                    NPCID.ZombieRaincoat,
                    NPCID.ZombieRaincoat,
                    NPCID.ZombieXmas,
                    NPCID.ZombieSweater,
                    NPCID.BloodZombie,
                    NPCID.ZombieDoctor,
                    NPCID.ZombieEskimo,
                    NPCID.ZombiePixie,
                    NPCID.ZombieSuperman,
                    NPCID.ArmedZombie,
                    NPCID.ArmedZombieCenx,
                    NPCID.ArmedZombieEskimo,
                    NPCID.ArmedZombiePincussion,
                    NPCID.ArmedZombieSlimed,
                    NPCID.ArmedZombieSwamp,
                    NPCID.ArmedZombieTwiggy
                };

                Corruption = new HashSet<int>()
                {
                    NPCID.DarkMummy,
                    NPCID.DesertDjinn,
                    NPCID.DesertLamiaDark,

                    NPCID.VileSpit,
                    NPCID.CursedHammer,
                    NPCID.SeekerHead,
                    NPCID.SeekerBody,
                    NPCID.SeekerTail,
                    NPCID.EaterofSouls,
                    NPCID.BigEater,
                    NPCID.LittleEater,
                    NPCID.EaterofWorldsHead,
                    NPCID.EaterofWorldsBody,
                    NPCID.EaterofWorldsTail,
                    NPCID.DevourerHead,
                    NPCID.DevourerBody,
                    NPCID.DevourerTail,
                    NPCID.Clinger,
                    NPCID.CorruptBunny,
                    NPCID.CorruptGoldfish,
                    NPCID.Corruptor,
                    NPCID.CorruptPenguin,
                    NPCID.CorruptSlime,
                    NPCID.Slimeling,
                    NPCID.Slimer,
                    NPCID.Slimer2,
                    NPCID.BigMimicCorruption,
                    NPCID.DesertGhoulCorruption,
                    NPCID.PigronCorruption,
                    NPCID.SandsharkCorrupt,
                };

                Crimson = new HashSet<int>()
                {
                    NPCID.DarkMummy,
                    NPCID.DesertDjinn,
                    NPCID.DesertLamiaDark,

                    NPCID.Crimera,
                    NPCID.FaceMonster,
                    NPCID.BloodCrawler,
                    NPCID.BloodCrawlerWall,
                    NPCID.Crimslime,
                    NPCID.Herpling,
                    NPCID.CrimsonGoldfish,
                    NPCID.BrainofCthulhu,
                    NPCID.Creeper,
                    NPCID.BloodJelly,
                    NPCID.BloodFeeder,
                    NPCID.CrimsonAxe,
                    NPCID.IchorSticker,
                    NPCID.FloatyGross,
                    NPCID.DesertGhoulCrimson,
                    NPCID.PigronCrimson,
                    NPCID.BigMimicCrimson,
                    NPCID.CrimsonBunny,
                    NPCID.CrimsonPenguin,
                };

                HotDamage = new HashSet<int>()
                {
                    NPCID.Hellbat,
                    NPCID.Lavabat,
                    NPCID.LavaSlime,
                    NPCID.BlazingWheel,
                    NPCID.FireImp,
                    NPCID.BurningSphere,
                    NPCID.MeteorHead,
                    NPCID.HellArmoredBones,
                    NPCID.HellArmoredBonesMace,
                    NPCID.HellArmoredBonesSpikeShield,
                    NPCID.HellArmoredBonesSword,
                    ModContent.NPCType<RedSprite>(),
                    ModContent.NPCType<WhiteSlime>(),
                };

                CannotBeMeathooked = new HashSet<int>()
                {
                    NPCID.WallofFlesh,
                    NPCID.WallofFleshEye,
                    NPCID.PlanterasHook,
                    NPCID.ForceBubble,
                };

                DealsLessDamageToCata = new HashSet<int>()
                {
                    NPCID.Mothron,
                    NPCID.MothronSpawn,
                    NPCID.MothronEgg,
                    NPCID.CultistBoss,
                    NPCID.CultistBossClone,
                    NPCID.AncientCultistSquidhead,
                    NPCID.CultistDragonHead,
                    NPCID.CultistDragonBody1,
                    NPCID.CultistDragonBody2,
                    NPCID.CultistDragonBody3,
                    NPCID.CultistDragonBody4,
                    NPCID.CultistDragonTail,
                    NPCID.AncientDoom,
                    NPCID.AncientLight
                };

                Unholy = new HashSet<int>()
                {
                    NPCID.Wraith,
                    NPCID.Hellhound,
                    NPCID.Scarecrow1, NPCID.Scarecrow2, NPCID.Scarecrow3, NPCID.Scarecrow4, NPCID.Scarecrow5, NPCID.Scarecrow6, NPCID.Scarecrow7, NPCID.Scarecrow8, NPCID.Scarecrow9, NPCID.Scarecrow10,
                    NPCID.Splinterling,
                    NPCID.MourningWood,
                    NPCID.VileSpit,
                    NPCID.Pumpking, NPCID.PumpkingBlade,
                };

                Unholy = AQUtils.Combine(Unholy, Corruption, Crimson);

                Hallowed = new HashSet<int>()
                {
                    NPCID.Pixie,
                    NPCID.Unicorn,
                    NPCID.EnchantedSword,
                    NPCID.RainbowSlime,
                    NPCID.Gastropod,
                    NPCID.LightMummy,
                    NPCID.BigMimicHallow,
                    NPCID.DesertGhoulHallow,
                    NPCID.PigronHallow,
                    NPCID.SandsharkHallow,
                    NPCID.ChaosElemental,
                };

                HecktoplasmSpawn = new HashSet<int>() 
                {
                    NPCID.DiabolistRed,
                    NPCID.DiabolistWhite,
                    NPCID.HellArmoredBones,
                    NPCID.HellArmoredBonesMace,
                    NPCID.HellArmoredBonesSpikeShield,
                    NPCID.HellArmoredBonesSword,
                };

                NoSpoilLoot = new HashSet<int>()
                {
                    NPCID.EaterofWorldsHead,
                    NPCID.EaterofWorldsBody,
                    NPCID.EaterofWorldsTail,
                    NPCID.Creeper,
                    NPCID.Mimic,
                    NPCID.BigMimicHallow,
                    NPCID.BigMimicCorruption,
                    NPCID.BigMimicCrimson,
                    NPCID.BigMimicJungle,
                    NPCID.DungeonGuardian,
                    NPCID.PresentMimic,
                    NPCID.LunarTowerNebula,
                    NPCID.LunarTowerSolar,
                    NPCID.LunarTowerStardust,
                    NPCID.LunarTowerVortex,
                    NPCID.MourningWood,
                    NPCID.Pumpking,
                    NPCID.Everscream,
                    NPCID.SantaNK1,
                    NPCID.IceQueen,
                };

                NoMapBlip = new HashSet<int>()
                {
                    NPCID.MartianSaucer,
                    NPCID.MartianSaucerCannon,
                    NPCID.MartianSaucerTurret,
                    NPCID.SpikeBall,
                    NPCID.BlazingWheel,
                    NPCID.ChaosBall,
                    NPCID.BurningSphere,
                    NPCID.ChaosBall,
                    NPCID.BurningSphere,
                    NPCID.WaterSphere,
                    NPCID.Spore,
                    NPCID.DetonatingBubble,
                    NPCID.ForceBubble,
                    NPCID.TargetDummy,
                };

                NoGlobalDrops = new HashSet<int>()
                {
                    NPCID.MeteorHead,
                    NPCID.ServantofCthulhu,
                    NPCID.ChaosBall,
                    NPCID.BurningSphere,
                    NPCID.SpikeBall,
                    NPCID.BlazingWheel,
                    NPCID.ShadowFlameApparition,
                    NPCID.VileSpit,
                    NPCID.BlueSlime,
                    NPCID.SlimeSpiked,
                    NPCID.TheHungry,
                    NPCID.TheHungryII,
                    NPCID.MoonLordLeechBlob,
                    NPCID.LeechHead,
                    NPCID.PlanterasTentacle,
                    NPCID.GolemFistLeft,
                    NPCID.GolemFistRight,
                    NPCID.GolemHead,
                    NPCID.VortexHornet,
                    NPCID.VortexHornetQueen,
                    NPCID.VortexLarva,
                    NPCID.VortexRifleman,
                    NPCID.VortexSoldier,
                    NPCID.StardustCellBig,
                    NPCID.StardustCellSmall,
                    NPCID.StardustJellyfishBig,
                    NPCID.StardustSoldier,
                    NPCID.StardustSpiderBig,
                    NPCID.StardustSpiderSmall,
                    NPCID.StardustWormHead,
                    NPCID.SolarCorite,
                    NPCID.SolarCrawltipedeHead,
                    NPCID.SolarCrawltipedeTail,
                    NPCID.SolarDrakomire,
                    NPCID.SolarDrakomireRider,
                    NPCID.SolarFlare,
                    NPCID.SolarGoop,
                    NPCID.SolarSolenian,
                    NPCID.SolarSroller,
                    NPCID.NebulaBeast,
                    NPCID.NebulaBrain,
                    NPCID.NebulaHeadcrab,
                    NPCID.NebulaSoldier,
                    NPCID.LunarTowerNebula,
                    NPCID.LunarTowerSolar,
                    NPCID.LunarTowerStardust,
                    NPCID.LunarTowerVortex,
                    NPCID.MartianDrone,
                    NPCID.MartianEngineer,
                    NPCID.MartianOfficer,
                    NPCID.MartianProbe,
                    NPCID.MartianSaucer,
                    NPCID.MartianSaucerCannon,
                    NPCID.MartianSaucerCore,
                    NPCID.MartianSaucerTurret,
                    NPCID.MartianTurret,
                    NPCID.MartianWalker,
                    NPCID.ForceBubble,
                    NPCID.AncientCultistSquidhead,
                    NPCID.AncientDoom,
                    NPCID.AncientLight,
                    NPCID.Creeper,
                    NPCID.Sharkron,
                    NPCID.Sharkron2,
                    NPCID.DetonatingBubble,
                    NPCID.DemonTaxCollector,
                    NPCID.DungeonSpirit,
                    ModContent.NPCType<Heckto>(),
                    NPCID.DungeonGuardian,
                    NPCID.Slimer,
                    NPCID.Bee,
                    NPCID.BeeSmall,
                    NPCID.CrimsonBunny,
                    NPCID.CrimsonGoldfish,
                    NPCID.CrimsonPenguin,
                    NPCID.BigMimicCorruption,
                    NPCID.BigMimicCrimson,
                    NPCID.BigMimicHallow,
                    NPCID.BigMimicJungle,
                    NPCID.CorruptBunny,
                    NPCID.CorruptGoldfish,
                    NPCID.CorruptPenguin,
                    NPCID.GoblinArcher,
                    NPCID.GoblinPeon,
                    NPCID.GoblinScout,
                    NPCID.GoblinSorcerer,
                    NPCID.GoblinSummoner,
                    NPCID.GoblinThief,
                    NPCID.GoblinWarrior,
                    NPCID.BoundGoblin,
                    NPCID.BoundMechanic,
                    NPCID.BoundWizard,
                    NPCID.PirateShip,
                    NPCID.PirateShipCannon,
                    NPCID.PirateCaptain,
                    NPCID.PirateCorsair,
                    NPCID.PirateCrossbower,
                    NPCID.PirateDeadeye,
                    NPCID.PirateDeckhand,
                    NPCID.Parrot,
                    NPCID.SnowmanGangsta,
                    NPCID.SnowBalla,
                    NPCID.MisterStabby,
                    NPCID.Mothron,
                    NPCID.MothronEgg,
                    NPCID.MothronSpawn,
                    NPCID.RayGunner,
                    NPCID.Scutlix,
                    NPCID.ScutlixRider,
                    NPCID.GrayGrunt,
                    NPCID.Scarecrow1,
                    NPCID.Scarecrow2,
                    NPCID.Scarecrow3,
                    NPCID.Scarecrow4,
                    NPCID.Scarecrow5,
                    NPCID.Scarecrow6,
                    NPCID.Scarecrow7,
                    NPCID.Scarecrow8,
                    NPCID.Scarecrow9,
                    NPCID.Scarecrow10,
                    NPCID.Splinterling,
                    NPCID.Hellhound,
                    NPCID.Poltergeist,
                    NPCID.MourningWood,
                    NPCID.Pumpking,
                    NPCID.ElfArcher,
                    NPCID.ElfCopter,
                    NPCID.ZombieElf,
                    NPCID.ZombieElfBeard,
                    NPCID.ZombieElfGirl,
                    NPCID.GingerbreadMan,
                    NPCID.Flocko,
                    NPCID.HeadlessHorseman,
                    NPCID.Nutcracker,
                    NPCID.NutcrackerSpinning,
                    NPCID.Yeti,
                    NPCID.Krampus,
                    NPCID.PresentMimic,
                    NPCID.Everscream,
                    NPCID.SantaNK1,
                    NPCID.IceQueen,
                    NPCID.PrimeCannon,
                    NPCID.PrimeLaser,
                    NPCID.PrimeSaw,
                    NPCID.PrimeVice,
                    NPCID.SkeletronHand,
                    NPCID.DD2EterniaCrystal,
                    NPCID.FungiSpore,
                    NPCID.Spore,
                    NPCID.CultistDevote,
                    NPCID.CultistArcherBlue,
                    NPCID.BartenderUnconscious,
                    NPCID.EaterofWorldsHead,
                    NPCID.EaterofWorldsBody,
                    NPCID.EaterofWorldsTail,
                    ModContent.NPCType<Meteor>(),
                    ModContent.NPCType<RedSprite>(),
                    ModContent.NPCType<SpaceSquid>(),
                };

                WindUpdates = new HashSet<int>()
                {
                    NPCID.KingSlime,
                    NPCID.EyeofCthulhu,
                    NPCID.BrainofCthulhu,
                    NPCID.EaterofWorldsHead,
                    NPCID.SkeletronHand,
                    NPCID.DevourerHead,
                    NPCID.GiantWormHead,
                    NPCID.Hornet,
                    NPCID.HornetFatty,
                    NPCID.HornetHoney,
                    NPCID.HornetLeafy,
                    NPCID.HornetSpikey,
                    NPCID.HornetStingy,
                    NPCID.ManEater,
                    NPCID.Snatcher,
                    NPCID.EaterofSouls,
                    NPCID.Corruptor,
                    NPCID.Crimera,
                    NPCID.IceElemental,
                    NPCID.AnomuraFungus,
                    NPCID.MushiLadybug,
                    NPCID.Duck2,
                    NPCID.DuckWhite2,
                    NPCID.DetonatingBubble,
                    NPCID.DungeonSpirit,
                    ModContent.NPCType<Heckto>(),
                    ModContent.NPCType<NPCs.Monsters.GlimmerMonsters.Starite>(),
                    ModContent.NPCType<TrapImp>(),
                    ModContent.NPCType<Cindera>(),
                    ModContent.NPCType<Meteor>(),
                    ModContent.NPCType<NPCs.Monsters.CrabCreviceMonsters.SoldierCrab>(),
                    ModContent.NPCType<BalloonMerchant>(),
                };
            }

            private void AutoSets()
            {
                for (int i = 0; i < NPCLoader.NPCCount; i++)
                {
                    if (NPCID.Sets.BelongsToInvasionOldOnesArmy[i])
                    {
                        continue;
                    }
                    try
                    {
                        NPC npc;
                        if (i > Main.maxNPCTypes)
                        {
                            npc = NPCLoader.GetNPC(i).npc;
                        }
                        else
                        {
                            npc = new NPC();
                            npc.SetDefaults(i);
                        }
                        if (npc.aiStyle == NPCAIStyles.DemonEyeAI || npc.aiStyle == NPCAIStyles.FlyingAI || npc.aiStyle == NPCAIStyles.SpellAI || npc.aiStyle == NPCAIStyles.EnchantedSwordAI || npc.aiStyle == NPCAIStyles.SpiderAI ||
                            (!npc.noGravity && !npc.boss))
                        {
                            WindUpdates.Add(npc.type);
                        }
                    }
                    catch (Exception e)
                    {
                        var l = AQMod.Instance.Logger;
                        string npcname;
                        if (i > Main.maxNPCTypes)
                        {
                            string tryName = Lang.GetNPCName(i).Value;
                            if (string.IsNullOrWhiteSpace(tryName) || tryName.StartsWith("Mods"))
                            {
                                npcname = NPCLoader.GetNPC(i).Name;
                            }
                            else
                            {
                                npcname = tryName + "/" + NPCLoader.GetNPC(i).Name;
                            }
                        }
                        else
                        {
                            npcname = Lang.GetNPCName(i).Value;
                        }
                        l.Error("An error occured when doing algorithmic checks for sets for {" + npcname + ", ID: " + i + "}");
                        l.Error(e.Message);
                        l.Error(e.StackTrace);
                    }
                }
            }
            private void CrossModPolarities()
            {
                TryAddTo(AQMod.polarities, "Esophage", Corruption, Crimson, Unholy);
                TryAddTo(AQMod.polarities, "EsophageHitbox", Corruption, Crimson, Unholy);
                TryAddTo(AQMod.polarities, "EsophageLeg", Corruption, Crimson, Unholy);
                TryAddTo(AQMod.polarities, "LightEater", Corruption, Unholy);
                TryAddTo(AQMod.polarities, "LivingSpineHead", Crimson, Unholy);
                TryAddTo(AQMod.polarities, "LivingSpineBody", Crimson, Unholy);
                TryAddTo(AQMod.polarities, "LivingSpineTail", Crimson, Unholy);
                TryAddTo(AQMod.polarities, "RavenousCursedHead", Corruption, Unholy);
                TryAddTo(AQMod.polarities, "RavenousCursedBody", Corruption, Unholy);
                TryAddTo(AQMod.polarities, "RavenousCursedTail", Corruption, Unholy);
                TryAddTo(AQMod.polarities, "ScytheFlier", Crimson, Unholy);
                TryAddTo(AQMod.polarities, "Uraraneid", Crimson, Unholy);
                TryAddTo(AQMod.polarities, "TendrilAmalgam", Corruption, Unholy);

                TryAddTo(AQMod.polarities, "SunPixie", Hallowed);
                TryAddTo(AQMod.polarities, "Aequorean", Hallowed);
                TryAddTo(AQMod.polarities, "IlluminantScourer", Hallowed);
                TryAddTo(AQMod.polarities, "Painbow", Hallowed);
                TryAddTo(AQMod.polarities, "SunKnight", Hallowed);
                TryAddTo(AQMod.polarities, "SunServitor", Hallowed);
                TryAddTo(AQMod.polarities, "Trailblazer", Hallowed);
            }
            private void CrossModSplit()
            {
                TryAddTo(AQMod.split, "Decaying", Corruption, Unholy);
                TryAddTo(AQMod.split, "Stalker", Crimson, Unholy);
                TryAddTo(AQMod.split, "Spotter", Corruption, Crimson, Unholy);

                TryAddTo(AQMod.split, "Echo", Hallowed);
                TryAddTo(AQMod.split, "Fairyfly", Hallowed);
                TryAddTo(AQMod.split, "ShinyPixie", Hallowed);
                TryAddTo(AQMod.split, "SkeletonJester", Hallowed);
            }
            public void SetupContent()
            {
                AutoSets();

                WindUpdates.Remove(NPCID.TargetDummy);
                WindUpdates.Remove(NPCID.BigMimicCorruption);
                WindUpdates.Remove(NPCID.BigMimicHallow);
                WindUpdates.Remove(NPCID.BigMimicJungle);
                WindUpdates.Remove(NPCID.MourningWood);
                WindUpdates.Remove(NPCID.Everscream);
                WindUpdates.Remove(NPCID.SantaNK1);
                WindUpdates.Remove(NPCID.CultistArcherBlue);
                WindUpdates.Remove(NPCID.CultistDevote);
                WindUpdates.Remove(NPCID.Antlion);
                WindUpdates.Remove(NPCID.Paladin);
                WindUpdates.Remove(NPCID.Yeti);
                WindUpdates.Remove(NPCID.Krampus);
                WindUpdates.Remove(NPCID.BrainScrambler);
                WindUpdates.Remove(NPCID.RayGunner);
                WindUpdates.Remove(NPCID.MartianOfficer);
                WindUpdates.Remove(NPCID.GrayGrunt);
                WindUpdates.Remove(NPCID.MartianEngineer);
                WindUpdates.Remove(NPCID.GigaZapper);
                WindUpdates.Remove(NPCID.Scutlix);
                WindUpdates.Remove(NPCID.ScutlixRider);
                WindUpdates.Remove(NPCID.StardustSoldier);
                WindUpdates.Remove(NPCID.StardustSpiderBig);
                WindUpdates.Remove(NPCID.StardustSpiderSmall);
                WindUpdates.Remove(NPCID.SolarDrakomire);
                WindUpdates.Remove(NPCID.SolarDrakomireRider);
                WindUpdates.Remove(NPCID.SolarSroller);
                WindUpdates.Remove(NPCID.SolarSolenian);
                WindUpdates.Remove(NPCID.NebulaBeast);
                WindUpdates.Remove(NPCID.NebulaSoldier);
                WindUpdates.Remove(NPCID.VortexRifleman);
                WindUpdates.Remove(NPCID.VortexSoldier);
                WindUpdates.Remove(NPCID.VortexLarva);
                WindUpdates.Remove(NPCID.VortexHornet);
                WindUpdates.Remove(NPCID.VortexHornetQueen);
                WindUpdates.Remove(NPCID.Nailhead);
                WindUpdates.Remove(NPCID.Eyezor);
                WindUpdates.Remove(NPCID.GoblinSummoner);
                WindUpdates.Remove(NPCID.SolarSpearman);
                WindUpdates.Remove(NPCID.MartianWalker);
                WindUpdates.Remove(ModContent.NPCType<HermitCrab>());

                if (AQMod.polarities.IsActive)
                {
                    CrossModPolarities();
                }
                if (AQMod.split.IsActive)
                {
                    CrossModSplit();
                }
            }
        }

        internal static Color GreenSlimeColor => new Color(0, 220, 40, 100);
        internal static Color BlueSlimeColor => new Color(0, 80, 255, 100);

        public static bool BossRush { get; private set; }
        public static byte BossRushPlayer { get; private set; }

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public int drainableManaMax;
        public int drainableMana;

        public bool shimmering;
        public bool blueFire;
        public bool lovestruck;
        public bool corruptHellfire;
        public bool crimsonHellfire;
        public bool minionHaunted;

        public bool hotDamage;

        public bool setLavaImmune;

        /// <summary>
        /// When this flag is raised, no wind events should be applied to this NPC
        /// </summary>
        public bool windStruck;
        public bool windStruckOld;

        public sbyte temperature;

        public int updateTick;

        private void UpdateTemperature(NPC npc)
        {
            if (hotDamage)
            {
                if (temperature < 100)
                    temperature++;
            }
            else if (npc.coldDamage)
            {
                if (temperature > -100)
                    temperature--;
            }
            else
            {
                if (temperature < -100)
                {
                    temperature = -100;
                }
                else if (temperature > 100)
                {
                    temperature = 100;
                }
            }
        }
        public override void ResetEffects(NPC npc)
        {
            UpdateTemperature(npc);
            shimmering = false;
            blueFire = false;
            windStruckOld = windStruck;
            windStruck = false;
            lovestruck = false;
            corruptHellfire = false;
            crimsonHellfire = false;
            minionHaunted = false;
            if (drainableMana < drainableManaMax && (updateTick % 4 == 0))
            {
                drainableMana++;
            }
            updateTick++;
        }

        private void CheckBuffImmunes(NPC npc)
        {
            if (Sets.Instance.Corruption.Contains(npc.netID))
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
            if (Sets.Instance.Crimson.Contains(npc.netID))
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.CrimsonHellfire>()] = true;

            npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.LovestruckAQ>()] = npc.buffImmune[BuffID.Lovestruck];

            if (npc.buffImmune[BuffID.CursedInferno] || npc.buffImmune[BuffID.ShadowFlame])
            {
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.BlueFire>()] = true;
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.Sparkling>()] = true;
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
            }
        }
        public override void SetDefaults(NPC npc)
        {
            if (!AQMod.Loading)
            {
                if (Sets.Instance.HotDamage.Contains(npc.netID))
                    hotDamage = true;
                CheckBuffImmunes(npc);
            }
            drainableMana = Math.Min(npc.lifeMax / 20, 100);
        }
        public void PostSetDefaults(NPC npc, int Type, float scaleOverride)
        {
            drainableManaMax = drainableMana;
            setLavaImmune = npc.lavaImmune;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (shimmering)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 3 * 8;
                if (damage < 3)
                    damage = 3;
            }
            if (blueFire)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 12 * 8;
                if (damage < 12)
                    damage = 12;
            }
            if (corruptHellfire || crimsonHellfire)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 15 * 8;
                if (damage < 15)
                    damage = 15;
            }
        }

        public override void AI(NPC npc)
        {
            if (!setLavaImmune && (npc.townNPC || npc.type == NPCID.SkeletonMerchant))
            {
                if (MiscWorldInfo.villagerLavaImmunity)
                {
                    npc.lavaImmune = true;
                }
                else
                {
                    npc.lavaImmune = false;
                }
            }
        }

        private void DrawEffects_MinionHaunted(NPC npc, ref Color c)
        {
            c.R = AQUtils.MultClamp(c.R, 2f, 60, 120);
            c.G = AQUtils.MultClamp(c.G, 0.2f);
            c.B = AQUtils.MultClamp(c.B, 0.2f);
            byte redMin = (byte)(c.R / 2);
            if (c.G > redMin)
            {
                c.G = redMin;
            }
            if (c.B > redMin)
            {
                c.B = redMin;
            }
            if (Main.rand.NextBool(30))
            {
                var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(60, 2, 10, 175));
                d.noGravity = true;
                d.noLight = true;
                d.velocity = new Vector2(npc.velocity.X * 0.4f, Math.Min(npc.velocity.Y, 1f + Main.rand.NextFloat(0.1f, 1f)));
            }
        }
        private void DrawEffects_Shimmering(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server && AQGraphics.GameWorldActive)
            {
                var center = npc.Center;
                var dustPos = npc.position + new Vector2(Main.rand.Next(npc.width + 4) - 2f, Main.rand.Next(npc.height + 4) - 2f);
                var normal = Vector2.Normalize(dustPos - center);
                float size = npc.Size.Length() / 2f;
                var velocity = normal * Main.rand.NextFloat(size / 12f, size / 6f);
                velocity += -npc.velocity * 0.3f;
                velocity *= 0.05f;

                float npcVelocityLength = npc.velocity.Length();
                AQMod.Particles.PostDrawPlayers.AddParticle(
                    new MonoParticle(dustPos, velocity,
                    new Color(0.9f, Main.rand.NextFloat(0.6f, 0.9f), Main.rand.NextFloat(0.4f, 1f), 0f), Main.rand.NextFloat(0.3f, 1f)));

                int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<UltimateEnergyDust>());
                Main.dust[d].velocity = (Main.dust[d].position - center) / 32f + npc.velocity * 0.1f;

                if (Main.rand.NextBool(30))
                {
                    dustPos = npc.position + new Vector2(Main.rand.Next(npc.width + 4) - 2f, Main.rand.Next(npc.height + 4) - 2f);
                    normal = Vector2.Normalize(dustPos - center);
                    velocity = normal * Main.rand.NextFloat(size / 12f, size / 6f);
                    velocity += -npc.velocity * 0.3f;
                    velocity *= 0.01f;

                    var sparkleClr = new Color(0.5f, Main.rand.NextFloat(0.1f, 0.5f), Main.rand.NextFloat(0.1f, 0.55f), 0f);
                    AQMod.Particles.PostDrawPlayers.AddParticle(
                        new BrightSparkle(dustPos, velocity,
                        sparkleClr, 1f));
                }
            }
            Lighting.AddLight(npc.Center, 0.25f, 0.25f, 0.25f);
        }
        private void DrawEffects_BlueFire(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server && AQGraphics.GameWorldActive)
            {
                int amount = (npc.width + npc.height) / 30;
                if (amount < 3)
                    amount = 3;
                for (int i = 0; i < amount; i++)
                {
                    var pos = npc.position - new Vector2(2f, 2f);
                    var rect = new Rectangle((int)pos.X, (int)pos.Y, npc.width + 4, npc.height + 4);
                    var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                    AQMod.Particles.PostDrawPlayers.AddParticle(
                        new EmberParticleSubtractColorUsingScale(dustPos, new Vector2((npc.velocity.X + Main.rand.NextFloat(-3f, 3f)) * 0.3f, ((npc.velocity.Y + Main.rand.NextFloat(-3f, 3f)) * 0.4f).Abs() - 2f),
                        new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.2f, 1.2f)));
                }
            }
            Lighting.AddLight(npc.Center, 0.4f, 0.4f, 1f);
        }
        private Color DrawEffects_EvilFire_DetermineColor()
        {
            Color fireColor = Buffs.Debuffs.CorruptionHellfire.FireColor;
            if (corruptHellfire && crimsonHellfire)
            {
                fireColor = Color.Lerp(fireColor, Buffs.Debuffs.CrimsonHellfire.FireColor, 0.5f);
            }
            else if (crimsonHellfire)
            {
                fireColor = Buffs.Debuffs.CrimsonHellfire.FireColor;
                fireColor.G = Math.Min((byte)(fireColor.G + Main.rand.Next(40)), (byte)255);
            }
            return fireColor;
        }
        private void DrawEffects_EvilFire(NPC npc)
        {
            var fireColor = DrawEffects_EvilFire_DetermineColor();
            if (Main.netMode != NetmodeID.Server && AQGraphics.GameWorldActive)
            {
                var pos = npc.position - new Vector2(2f, 2f);
                var rect = new Rectangle((int)pos.X, (int)pos.Y, npc.width + 4, npc.height + 4);
                int amt = (int)(npc.Size.Length() / 16);
                amt += 4;
                for (int i = 0; i < amt; i++)
                {
                    var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                    var velocity = new Vector2((npc.velocity.X + Main.rand.NextFloat(-3f, 3f)) * 0.3f, ((npc.velocity.Y + Main.rand.NextFloat(-1f, 4f)) * 0.425f).Abs() - 3f);
                    AQMod.Particles.PostDrawPlayers.AddParticle(
                        new EmberParticleSubtractColorUsingScale(dustPos, velocity,
                        fireColor, Main.rand.NextFloat(0.7f, 0.95f)));
                }
            }
            Lighting.AddLight(npc.Center, fireColor.ToVector3());
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (minionHaunted)
            {
                DrawEffects_MinionHaunted(npc, ref drawColor);
            }
            if (shimmering)
            {
                DrawEffects_Shimmering(npc);
            }
            if (blueFire)
            {
                DrawEffects_BlueFire(npc);
            }
            if (corruptHellfire || crimsonHellfire)
            {
                DrawEffects_EvilFire(npc);
            }
        }

        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            if (npc.life < 0 && npc.HasValidTarget && Main.myPlayer == npc.target && npc.CanBeChasedBy(Main.player[npc.target]))
            {
                if (shimmering)
                {
                    int amount = (int)(100 * AQConfigClient.Instance.EffectIntensity);
                    if (AQConfigClient.Instance.EffectQuality < 1f)
                        amount = (int)(amount * AQConfigClient.Instance.EffectQuality);
                    if (AQConfigClient.Instance.Screenshakes)
                    {
                        FX.AddShake(AQGraphics.MultIntensity(12), 24f, 6f);
                    }
                    var npcCenter = npc.Center;
                    int p = Projectile.NewProjectile(npcCenter, Vector2.Normalize(npcCenter - Main.player[npc.target].Center), ModContent.ProjectileType<SparklingExplosion>(), 50, 5f, npc.target);
                    var size = Main.projectile[p].Size;
                    float radius = size.Length() / 5f;
                    for (int i = 0; i < amount; i++)
                    {
                        var offset = new Vector2(Main.rand.NextFloat(radius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                        var normal = Vector2.Normalize(offset);
                        var dustPos = npcCenter + offset;
                        var velocity = normal * Main.rand.NextFloat(6f, 12f);
                        AQMod.Particles.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.9f, Main.rand.NextFloat(0.7f, 0.9f), Main.rand.NextFloat(0.4f, 1f), 0f), Main.rand.NextFloat(0.8f, 1.1f)));
                        AQMod.Particles.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.9f, Main.rand.NextFloat(0.7f, 0.9f), Main.rand.NextFloat(0.4f, 1f), 0f) * 0.2f, 1.5f));
                        if (Main.rand.NextBool(14))
                        {
                            var sparkleClr = new Color(0.9f, Main.rand.NextFloat(0.8f, 0.9f), Main.rand.NextFloat(0.4f, 1f), 0f);
                            AQMod.Particles.PostDrawPlayers.AddParticle(
                                new SparkleParticle(dustPos, velocity,
                                sparkleClr, 1.5f));
                            AQMod.Particles.PostDrawPlayers.AddParticle(
                                new SparkleParticle(dustPos, velocity,
                                sparkleClr * 0.5f, 1f)
                                { rotation = MathHelper.PiOver4 });
                        }
                    }
                }
            }
        }

        private void ApplyDamageEffects(ref int damage)
        {
            if (lovestruck)
                damage += (int)(damage * 0.1f);
        }
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            ApplyDamageEffects(ref damage);
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            var aQNPC = npc.GetGlobalNPC<AQNPC>();
            if (aQNPC.minionHaunted && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || AQProjectile.Sets.Instance.IsAMinionProj.Contains(projectile.type)))
            {
                float multiplier = 2f;
                if (npc.boss)
                {
                    multiplier = 1.2f;
                }
                damage = (int)(damage * multiplier);
            }
            ApplyDamageEffects(ref damage);
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (lovestruck)
                damage -= (int)(damage * 0.1f);
        }

        public override bool PreAI(NPC npc)
        {
            if (MoonlightWallManager.CurrentlyCachingDaytimeFlag) // in case the NPC before this one broke and skipped PostAI, if there's a next NPC then it would hopefully fix it
                MoonlightWallManager.End();
            if (MoonlightWallManager.BehindMoonlightWall(npc.Center))
                MoonlightWallManager.Begin();
            if (npc.aiStyle == 13 && npc.ai[0] == 0 && npc.ai[1] == 0)
            {
                Point pos = npc.Center.ToTileCoordinates();
                if (pos.Y < 10 || pos.Y > Main.maxTilesX - 10)
                    return true;
                var t = Framing.GetTileSafely(pos.X, pos.Y);
                if (t.active() && Main.tileSolid[t.type] && !Main.tileSolidTop[t.type])
                {
                    npc.ai[0] = npc.Center.ToTileCoordinates().X;
                    npc.ai[1] = npc.Center.ToTileCoordinates().Y;
                    return true;
                }
                for (int j = pos.Y - 5; j < pos.Y + 5; j++)
                {
                    t = Framing.GetTileSafely(pos.X, j);
                    if (j != 0 && t.active() && Main.tileSolid[t.type] && !Main.tileSolidTop[t.type])
                    {
                        npc.ai[0] = pos.X;
                        npc.ai[1] = j;
                        return true;
                    }
                }
                npc.netUpdate = true;
            }
            return true;
        }

        public override void PostAI(NPC npc)
        {
            if (MoonlightWallManager.CurrentlyCachingDaytimeFlag)
                MoonlightWallManager.End();
        }

        public override void SetupTravelShop(int[] shop, ref int nextSlot)
        {
            int cursorDye = -1;
            if (Main.player[Main.myPlayer].statLifeMax >= 400 && Main.rand.NextBool(Main.hardMode ? 8 : 4))
                cursorDye = ModContent.ItemType<HealthCursorDye>();
            if (Main.player[Main.myPlayer].statManaMax >= 200 && Main.rand.NextBool(Main.hardMode ? 9 : 5))
                cursorDye = ModContent.ItemType<ManaCursorDye>();
            if (cursorDye != -1)
            {
                shop[nextSlot] = ModContent.ItemType<CursorDyeRemover>();
                shop[nextSlot + 1] = cursorDye;
                nextSlot += 2;
            }
        }

        private static float averageScale(int lifeMax, bool boss = false)
        {
            if (boss)
            {
                if (lifeMax >= 350000)
                    return 1.25f;
                if (lifeMax >= 100000)
                    return 1.333f;
                if (lifeMax >= 50000)
                    return 1.5f;
                if (lifeMax >= 20000)
                    return 1.75f;
                return 2f;
            }
            if (lifeMax >= 5000)
                return 1.2f;
            if (lifeMax >= 2500)
                return 1.333f;
            if (lifeMax >= 1000)
                return 1.4f;
            return 1.5f;
        }

        private static float bossRushScale(int lifeMax)
        {
            return MathHelper.Clamp(averageScale((int)(lifeMax * 3.5), true) * 0.6f, 1.05f, 1.25f);
        }

        public override bool PreNPCLoot(NPC npc)
        {
            if (LootLoopingHelper.Current != 0)
            {
                NPCLoader.blockLoot.Add(ItemID.Heart);
                NPCLoader.blockLoot.Add(ItemID.Star);
            }
            return true;
        }

        private void FeatherFlightAmuletCheck(NPC npc, List<(Player, AQPlayer)> nearbyPlayers)
        {
            int wingTime = npc.lifeMax / (Main.expertMode ? 6 : 2);
            foreach (var p in nearbyPlayers)
            {
                if (p.Item2.featherflightAmulet)
                {
                    p.Item1.wingTime += wingTime;
                    if (p.Item1.wingTime > p.Item1.wingTimeMax)
                    {
                        p.Item1.wingTime = p.Item1.wingTimeMax;
                    }
                }
            }
        }
        private void BloodthirstPotionCheck(NPC npc, List<(Player, AQPlayer)> nearbyPlayers)
        {
            int healAmount = npc.lifeMax / 1000 + 5;
            foreach (var p in nearbyPlayers)
            {
                if (!p.Item1.moonLeech && p.Item2.bloodthirstCooldown == 0 && p.Item2.bloodthirst)
                {
                    p.Item2.bloodthirstCooldown = 255;
                    AQPlayer.HealPlayer(p.Item1, healAmount, broadcast: true, mergeHealEffect: true, AQUtils.Instance(ModContent.ItemType<BloodthirstPotion>()),
                        healingItemQuickHeal: false);
                }
            }
        }
        private void BreadsoulCheck(NPC npc, List<(Player, AQPlayer)> nearbyPlayers)
        {
            int breadsoul = -1;
            for (int i = 0; i < nearbyPlayers.Count; i++)
            {
                var plr = nearbyPlayers[i];
                if (plr.Item2.breadSoul)
                {
                    breadsoul = i;
                    break;
                }
            }
            if (breadsoul == -1)
                return;
            for (int i = 0; i < 40; i++)
            {
                var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(200, 200, 255, 100));
                d.velocity = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToRotationVector2() * Main.rand.NextFloat(3f, 7f);
            }
            var center = npc.Center;
            float rot = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            var n = rot.ToRotationVector2();
            int p = Projectile.NewProjectile(npc.Center + n * 10f, n * 4.5f, ModContent.ProjectileType<BreadsoulHealing>(), 0, 0f, breadsoul);
            Main.projectile[p].rotation = rot;
            for (int j = 0; j < 4; j++)
            {
                var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoSparkleDust>(), 0f, 0f, 0, new Color(200, 200, 255, 100));
                d.velocity = n.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(4f, 7.5f);
            }
        }
        private void DreadsoulCheck(NPC npc, List<(Player, AQPlayer)> nearbyPlayers)
        {
            int dreadsoul = -1;
            for (int i = 0; i < nearbyPlayers.Count; i++)
            {
                var plr = nearbyPlayers[i];
                if (plr.Item2.dreadSoul)
                {
                    dreadsoul = i;
                    break;
                }
            }
            if (dreadsoul == -1)
                return;
            for (int i = 0; i < 40; i++)
            {
                var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(200, 20, 50, 100));
                d.velocity = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToRotationVector2() * Main.rand.NextFloat(3f, 7f);
            }
            var center = npc.Center;
            for (int i = 0; i < 4; i++)
            {
                float rot = MathHelper.PiOver2 * i;
                rot += Main.rand.NextFloat(-0.2f, 0.2f);
                var n = rot.ToRotationVector2();
                int p = Projectile.NewProjectile(center + n * 10f, n * 7f, ModContent.ProjectileType<DreadsoulAttack>(), 10, 0f, dreadsoul);
                Main.projectile[p].rotation = rot;
                for (int j = 0; j < 4; j++)
                {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoSparkleDust>(), 0f, 0f, 0, new Color(200, 20, 50, 100));
                    d.velocity = n.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(4f, 7.5f);
                }
            }
        }
        private void RefillAmmoStocks(NPC npc, List<(Player, AQPlayer)> nearbyPlayers)
        {
            foreach (var p in nearbyPlayers)
            {
                if (p.Item2.ammoRenewal)
                {
                    Dictionary<int, int> returns = new Dictionary<int, int>();
                    foreach (var ammo in p.Item2.AmmoUsage)
                    {
                        int amtToReturn = ammo.Value;
                        amtToReturn = Main.rand.Next(amtToReturn) + 1;
                        returns.Add(ammo.Key, amtToReturn);
                        AQItem.DropInstancedItem(p.Item1.whoAmI, npc.getRect(), ammo.Key, amtToReturn);
                    }
                    foreach (var ammo in returns)
                    {
                        p.Item2.AmmoUsage[ammo.Key] -= ammo.Value;
                        if (p.Item2.AmmoUsage[ammo.Key] <= 0)
                        {
                            p.Item2.AmmoUsage.Remove(ammo.Key);
                        }
                    }
                }
            }
        }
        public override void NPCLoot(NPC npc)
        {
            if (npc.SpawnedFromStatue || NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type] || npc.lifeMax < 5 || npc.friendly)
                return;
            if (LootLoopingHelper.Current == 0 && (Main.netMode == NetmodeID.Server || !Main.gameMenu) && !Sets.Instance.NoGlobalDrops.Contains(npc.netID))
            {
                List<(Player, AQPlayer)> nearbyPlayers = new List<(Player, AQPlayer)>();
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    var player = Main.LocalPlayer;
                    nearbyPlayers.Add((player, player.GetModPlayer<AQPlayer>()));
                }
                else
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead && Main.player[i].DistanceSQ(npc.Center) < 2000f)
                        {
                            nearbyPlayers.Add((Main.player[i], Main.player[i].GetModPlayer<AQPlayer>()));
                        }
                    }
                }
                RefillAmmoStocks(npc, nearbyPlayers);
                BreadsoulCheck(npc, nearbyPlayers);
                DreadsoulCheck(npc, nearbyPlayers);
                FeatherFlightAmuletCheck(npc, nearbyPlayers);
                BloodthirstPotionCheck(npc, nearbyPlayers);
            }
        }

        public bool ShouldApplyWindMechanics(NPC npc)
        {
            return !windStruck && Sets.Instance.WindUpdates.Contains(npc.type);
        }

        public void ApplyWindMechanics(NPC npc, Vector2 wind)
        {
            npc.velocity += wind;
            windStruck = true;
        }

        public static bool AreTheSameNPC(int type, int otherType)
        {
            if (type == otherType)
                return true;
            int banner = Item.NPCtoBanner(type);
            if (banner != 0 && banner == Item.NPCtoBanner(otherType))
                return true;
            switch (type)
            {
                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    {
                        switch (otherType)
                        {
                            case NPCID.EaterofWorldsHead:
                            case NPCID.EaterofWorldsBody:
                            case NPCID.EaterofWorldsTail:
                                return true;
                        }
                    }
                    break;

                case NPCID.TheDestroyer:
                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                    {
                        switch (otherType)
                        {
                            case NPCID.TheDestroyer:
                            case NPCID.TheDestroyerBody:
                            case NPCID.TheDestroyerTail:
                                return true;
                        }
                    }
                    break;

                case NPCID.TheHungry:
                case NPCID.TheHungryII:
                    {
                        switch (otherType)
                        {
                            case NPCID.TheHungry:
                            case NPCID.TheHungryII:
                                return true;
                        }
                    }
                    break;

                case NPCID.Golem:
                case NPCID.GolemHead:
                case NPCID.GolemHeadFree:
                case NPCID.GolemFistLeft:
                case NPCID.GolemFistRight:
                    {
                        switch (otherType)
                        {
                            case NPCID.Golem:
                            case NPCID.GolemHead:
                            case NPCID.GolemHeadFree:
                            case NPCID.GolemFistLeft:
                            case NPCID.GolemFistRight:
                                return true;
                        }
                    }
                    break;
            }
            return false;
        }

        internal static int FindTarget(Vector2 position, float distance = 2000f)
        {
            int npc = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy())
                {
                    float dist = (Main.npc[i].Center - position).Length();
                    if (dist < distance)
                    {
                        npc = i;
                        distance = dist;
                    }
                }
            }
            return npc;
        }

        public static bool ConvertNPCtoGold(int i)
        {
            switch (Main.npc[i].type)
            {
                case NPCID.Bunny:
                case NPCID.BunnySlimed:
                case NPCID.BunnyXmas:
                case NPCID.PartyBunny:
                    {
                        Main.npc[i].Transform(NPCID.GoldBunny);
                    }
                    return true;

                case NPCID.Squirrel:
                case NPCID.SquirrelRed:
                    {
                        Main.npc[i].Transform(NPCID.SquirrelGold);
                    }
                    return true;

                case NPCID.Bird:
                case NPCID.BirdBlue:
                case NPCID.BirdRed:
                    {
                        Main.npc[i].Transform(NPCID.GoldBird);
                    }
                    return true;

                case NPCID.Butterfly:
                    {
                        Main.npc[i].Transform(NPCID.GoldButterfly);
                    }
                    return true;

                case NPCID.Frog:
                    {
                        Main.npc[i].Transform(NPCID.GoldFrog);
                    }
                    return true;

                case NPCID.Grasshopper:
                    {
                        Main.npc[i].Transform(NPCID.GoldGrasshopper);
                    }
                    return true;

                case NPCID.Mouse:
                    {
                        Main.npc[i].Transform(NPCID.GoldMouse);
                    }
                    return true;

                case NPCID.Worm:
                    {
                        Main.npc[i].Transform(NPCID.GoldWorm);
                    }
                    return true;
            }
            return false;
        }

        public static void CollideWithNPCs(Action<NPC> onCollide, Rectangle myRect)
        {
            CollideWithNPCs(onCollide, (n) => new Rectangle((int)n.position.X, (int)n.position.Y, n.width, n.height).Intersects(myRect));
        }

        public static void CollideWithNPCs(Action<NPC> onCollide, Func<NPC, bool> isColliding)
        {
            for (int i = 0; i < 200; i++)
            {
                if (!Main.npc[i].active)
                    continue;
                if (isColliding(Main.npc[i]))
                    onCollide(Main.npc[i]);
            }
        }

        public static void ImmuneToAllBuffs(NPC NPC)
        {
            for (int i = 0; i < NPC.buffImmune.Length; i++)
            {
                NPC.buffImmune[i] = true;
            }
        }

        public static bool UselessNPC(NPC npc)
        {
            return npc.friendly || npc.lifeMax < 5;
        }

        public static bool CanBeMeathooked(NPC npc)
        {
            return !UselessNPC(npc) && !npc.dontTakeDamage && !npc.immortal && !Sets.Instance.CannotBeMeathooked.Contains(npc.type);
        }

        public void ChangeTemperature(NPC npc, sbyte newTemperature)
        {
            if (hotDamage && newTemperature > 0)
            {
                newTemperature /= 2;
            }
            else if (npc.coldDamage && newTemperature < 0)
            {
                newTemperature /= 2;
            }
            if (temperature < 0)
            {
                if (newTemperature < 0)
                {
                    if (temperature > newTemperature)
                        temperature = newTemperature;
                }
                else
                {
                    temperature = 0;
                }
            }
            else if (temperature > 0)
            {
                if (newTemperature > 0)
                {
                    if (temperature < newTemperature)
                        temperature = newTemperature;
                }
                else
                {
                    temperature = 0;
                }
            }
            else
            {
                temperature = newTemperature;
            }
            if (newTemperature < 0)
            {
                temperature--;
            }
            else
            {
                temperature++;
            }
        }

        public static int CountNPCs(bool[] ruleset)
        {
            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && ruleset[Main.npc[i].type])
                    count++;
            }
            return count;
        }

        public static int CountNPCs(params bool[][] ruleset)
        {
            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                foreach (bool[] b in ruleset)
                {
                    if (Main.npc[i].active && b[Main.npc[i].type])
                    {
                        count++;
                        break;
                    }
                }
            }
            return count;
        }
    }
}