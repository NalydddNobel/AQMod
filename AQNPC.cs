using AQMod.Common.Graphics;
using AQMod.Common.Graphics.Particles;
using AQMod.Common.NoHitting;
using AQMod.Content;
using AQMod.Content.Players;
using AQMod.Content.Quest.Lobster;
using AQMod.Content.World.Events;
using AQMod.Effects.ScreenEffects;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.Amulets;
using AQMod.Items.Foods;
using AQMod.Items.Foods.Dungeon;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Tools;
using AQMod.Items.Tools.MapMarkers;
using AQMod.Items.Vanities.CursorDyes;
using AQMod.Items.Weapons.Melee.Dagger;
using AQMod.NPCs;
using AQMod.NPCs.Friendly;
using AQMod.NPCs.Monsters;
using AQMod.NPCs.Monsters.DemonSiege;
using AQMod.NPCs.Monsters.GaleStreams;
using AQMod.Projectiles;
using AQMod.Projectiles.Monster.Starite;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQNPC : GlobalNPC
    {
        internal static class AIStyles // personal ai style list
        {
            public const int BoundNPCAI = 0;
            public const int SlimeAI = 1;
            public const int DemonEyeAI = 2;
            public const int FighterAI = 3;
            public const int EoCAI = 4;
            public const int FlyingAI = 5;
            public const int WormAI = 6;
            public const int PassiveAI = 7;
            public const int CasterAI = 8;
            public const int SpellAI = 9;
            public const int CursedSkullAI = 10;
            public const int SkeletronAI = 11;
            public const int SkeletronHandAI = 12;
            public const int ManEaterAI = 13;
            public const int BatAI = 14;
            public const int KingSlimeAI = 15;
            public const int FishAI = 16;
            public const int VultureAI = 17;
            public const int JellyfishAI = 18;
            public const int AntlionAI = 19;
            public const int SpikeBallAI = 20;
            public const int BlazingWheelAI = 21;
            public const int HoveringAI = 22;
            public const int EnchantedSwordAI = 23;
            public const int BirdCritterAI = 24;
            public const int MimicAI = 25;
            public const int UnicornAI = 26;
            public const int WoFAI = 27;
            public const int TheHungryAI = 28;

            public const int SnowmanAI = 38;
            public const int TortiseAI = 39;
            public const int SpiderAI = 40;
            public const int DerplingAI = 41;

            public const int FlyingFishAI = 44;

            public const int AngryNimbusAI = 49;
            public const int SporeAI = 50;

            public const int DungeonSpiritAI = 56;
            public const int MourningWoodAI = 57;
            public const int PumpkingAI = 58;
            public const int PumpkingScytheAI = 59;
            public const int IceQueenAI = 60;
            public const int SantaNKIAI = 61;
            public const int ElfCopterAI = 62;
            public const int FlockoAI = 63;
            public const int FireflyAI = 64;
            public const int ButterflyAI = 65;
            public const int WormCritterAI = 66;
            public const int SnailAI = 67;
            public const int DuckAI = 68;
            public const int DukeFishronAI = 69;

            public const int BiomeMimicAI = 87;
            public const int MothronAI = 88;

            public const int GraniteElementalAI = 91;

            public const int SandElementalAI = 102;
            public const int SandSharkAI = 103;
        }

        public static class Sets
        {
            public static bool[] NoSpoilLoot { get; private set; }
            public static bool[] NoMapBlip { get; private set; }
            public static bool[] NoGlobalDrops { get; private set; }
            public static bool[] HecktoplasmDungeonEnemy { get; private set; }
            public static bool[] EnemyDungeonSprit { get; private set; }
            public static bool[] DemonSiegeEnemy { get; private set; }
            public static bool[] UnaffectedByWind { get; private set; }
            public static bool[] BossRelatedEnemy { get; private set; }

            public static bool IsASkeleton(int type, bool includeHardmodeSkeletons = true, bool includeHoplites = true)
            {
                if (includeHardmodeSkeletons && (type == NPCID.ArmoredSkeleton || type == NPCID.SkeletonArcher))
                    return true;
                if (includeHoplites && type == NPCID.GreekSkeleton)
                    return true;
                return type == NPCID.Skeleton ||
                    type == NPCID.SkeletonAlien ||
                    type == NPCID.SkeletonAstonaut ||
                    type == NPCID.SkeletonTopHat ||
                    type == NPCID.HeadacheSkeleton ||
                    type == NPCID.MisassembledSkeleton ||
                    type == NPCID.PantlessSkeleton ||
                    type == NPCID.BoneThrowingSkeleton ||
                    type == NPCID.BoneThrowingSkeleton2 ||
                    type == NPCID.BoneThrowingSkeleton3 ||
                    type == NPCID.BoneThrowingSkeleton4 ||
                    type == NPCID.GreekSkeleton;
            }

            public static bool IsAZombie(int type, bool includeBloodZombies = true)
            {
                if (includeBloodZombies && type == NPCID.BloodZombie)
                    return true;
                return type == NPCID.Zombie ||
                type == NPCID.BaldZombie ||
                type == NPCID.PincushionZombie ||
                type == NPCID.SlimedZombie ||
                type == NPCID.SwampZombie ||
                type == NPCID.TwiggyZombie ||
                type == NPCID.FemaleZombie ||
                type == NPCID.ZombieRaincoat ||
                type == NPCID.ZombieXmas ||
                type == NPCID.ZombieSweater;
            }

            internal static void LoadSets()
            {
                BossRelatedEnemy = new bool[NPCLoader.NPCCount];
                BossRelatedEnemy[NPCID.ServantofCthulhu] = true;
                BossRelatedEnemy[NPCID.PirateShipCannon] = true;
                BossRelatedEnemy[NPCID.MartianSaucerCannon] = true;
                BossRelatedEnemy[NPCID.EaterofWorldsHead] = true;
                BossRelatedEnemy[NPCID.EaterofWorldsBody] = true;
                BossRelatedEnemy[NPCID.EaterofWorldsTail] = true;
                BossRelatedEnemy[NPCID.TheDestroyerBody] = true;
                BossRelatedEnemy[NPCID.TheDestroyerTail] = true;
                BossRelatedEnemy[NPCID.MoonLordHand] = true;
                BossRelatedEnemy[NPCID.MoonLordHead] = true;
                BossRelatedEnemy[NPCID.GolemFistLeft] = true;
                BossRelatedEnemy[NPCID.GolemFistRight] = true;
                BossRelatedEnemy[NPCID.GolemHead] = true;
                BossRelatedEnemy[NPCID.PrimeVice] = true;
                BossRelatedEnemy[NPCID.MourningWood] = true;
                BossRelatedEnemy[NPCID.Pumpking] = true;
                BossRelatedEnemy[NPCID.Everscream] = true;
                BossRelatedEnemy[NPCID.SantaNK1] = true;
                BossRelatedEnemy[NPCID.IceQueen] = true;
                BossRelatedEnemy[NPCID.Paladin] = true;
                BossRelatedEnemy[NPCID.WyvernHead] = true;
                BossRelatedEnemy[NPCID.WyvernBody] = true;
                BossRelatedEnemy[NPCID.WyvernBody2] = true;
                BossRelatedEnemy[NPCID.WyvernBody3] = true;
                BossRelatedEnemy[NPCID.WyvernTail] = true;
                BossRelatedEnemy[NPCID.DD2DarkMageT1] = true;
                BossRelatedEnemy[NPCID.DD2DarkMageT3] = true;
                BossRelatedEnemy[NPCID.DD2OgreT2] = true;
                BossRelatedEnemy[NPCID.DD2OgreT3] = true;
                BossRelatedEnemy[NPCID.DD2Betsy] = true;
                BossRelatedEnemy[NPCID.Creeper] = true;
                BossRelatedEnemy[NPCID.BeeSmall] = true;
                BossRelatedEnemy[NPCID.Bee] = true;

                DemonSiegeEnemy = new bool[NPCLoader.NPCCount];
                DemonSiegeEnemy[ModContent.NPCType<Magmalbubble>()] = true;
                DemonSiegeEnemy[ModContent.NPCType<TrapImp>()] = true;
                DemonSiegeEnemy[ModContent.NPCType<Cindera>()] = true;

                EnemyDungeonSprit = new bool[NPCLoader.NPCCount];
                EnemyDungeonSprit[NPCID.DungeonSpirit] = true;
                EnemyDungeonSprit[ModContent.NPCType<Heckto>()] = true;

                HecktoplasmDungeonEnemy = new bool[NPCLoader.NPCCount];
                HecktoplasmDungeonEnemy[NPCID.DiabolistRed] = true;
                HecktoplasmDungeonEnemy[NPCID.DiabolistWhite] = true;
                HecktoplasmDungeonEnemy[NPCID.HellArmoredBones] = true;
                HecktoplasmDungeonEnemy[NPCID.HellArmoredBonesMace] = true;
                HecktoplasmDungeonEnemy[NPCID.HellArmoredBonesSpikeShield] = true;
                HecktoplasmDungeonEnemy[NPCID.HellArmoredBonesSword] = true;

                NoSpoilLoot = new bool[NPCLoader.NPCCount];
                NoSpoilLoot[NPCID.EaterofWorldsHead] = true;
                NoSpoilLoot[NPCID.EaterofWorldsBody] = true;
                NoSpoilLoot[NPCID.EaterofWorldsTail] = true;
                NoSpoilLoot[NPCID.Creeper] = true;
                NoSpoilLoot[NPCID.Mimic] = true;
                NoSpoilLoot[NPCID.BigMimicHallow] = true;
                NoSpoilLoot[NPCID.BigMimicCorruption] = true;
                NoSpoilLoot[NPCID.BigMimicCrimson] = true;
                NoSpoilLoot[NPCID.BigMimicJungle] = true;
                NoSpoilLoot[NPCID.DungeonGuardian] = true;
                NoSpoilLoot[NPCID.PresentMimic] = true;
                NoSpoilLoot[NPCID.Nailhead] = true;
                NoSpoilLoot[NPCID.TheGroom] = true;
                NoSpoilLoot[NPCID.TheBride] = true;
                NoSpoilLoot[NPCID.IceGolem] = true;
                NoSpoilLoot[NPCID.SandElemental] = true;
                NoSpoilLoot[NPCID.LunarTowerNebula] = true;
                NoSpoilLoot[NPCID.LunarTowerSolar] = true;
                NoSpoilLoot[NPCID.LunarTowerStardust] = true;
                NoSpoilLoot[NPCID.LunarTowerVortex] = true;
                NoSpoilLoot[NPCID.GoblinSummoner] = true;
                NoSpoilLoot[NPCID.PirateShip] = true;
                NoSpoilLoot[NPCID.Mothron] = true;
                NoSpoilLoot[NPCID.MourningWood] = true;
                NoSpoilLoot[NPCID.Pumpking] = true;
                NoSpoilLoot[NPCID.Everscream] = true;
                NoSpoilLoot[NPCID.SantaNK1] = true;
                NoSpoilLoot[NPCID.IceQueen] = true;

                NoMapBlip = new bool[NPCLoader.NPCCount];
                NoMapBlip[NPCID.MartianSaucer] = true;
                NoMapBlip[NPCID.MartianSaucerCannon] = true;
                NoMapBlip[NPCID.MartianSaucerTurret] = true;
                NoMapBlip[NPCID.SpikeBall] = true;
                NoMapBlip[NPCID.BlazingWheel] = true;
                NoMapBlip[NPCID.ChaosBall] = true;
                NoMapBlip[NPCID.BurningSphere] = true;
                NoMapBlip[NPCID.WaterSphere] = true;
                NoMapBlip[NPCID.Spore] = true;
                NoMapBlip[NPCID.DetonatingBubble] = true;
                NoMapBlip[NPCID.ForceBubble] = true;
                NoMapBlip[NPCID.FungiSpore] = true;
                NoMapBlip[NPCID.TargetDummy] = true;

                NoGlobalDrops = new bool[NPCLoader.NPCCount];
                NoGlobalDrops[NPCID.MeteorHead] = true;
                NoGlobalDrops[NPCID.ServantofCthulhu] = true;
                NoGlobalDrops[NPCID.ChaosBall] = true;
                NoGlobalDrops[NPCID.BurningSphere] = true;
                NoGlobalDrops[NPCID.SpikeBall] = true;
                NoGlobalDrops[NPCID.BlazingWheel] = true;
                NoGlobalDrops[NPCID.ShadowFlameApparition] = true;
                NoGlobalDrops[NPCID.Probe] = true;
                NoGlobalDrops[NPCID.VileSpit] = true;
                NoGlobalDrops[NPCID.BlueSlime] = true;
                NoGlobalDrops[NPCID.SlimeSpiked] = true;
                NoGlobalDrops[NPCID.TheHungry] = true;
                NoGlobalDrops[NPCID.TheHungryII] = true;
                NoGlobalDrops[NPCID.LeechHead] = true;
                NoGlobalDrops[NPCID.MoonLordLeechBlob] = true;
                NoGlobalDrops[NPCID.PlanterasHook] = true;
                NoGlobalDrops[NPCID.PlanterasTentacle] = true;
                NoGlobalDrops[NPCID.GolemFistLeft] = true;
                NoGlobalDrops[NPCID.GolemFistRight] = true;
                NoGlobalDrops[NPCID.GolemHead] = true;
                NoGlobalDrops[NPCID.GolemHeadFree] = true;
                NoGlobalDrops[NPCID.VortexSoldier] = true;
                NoGlobalDrops[NPCID.LunarTowerVortex] = true;
                NoGlobalDrops[NPCID.LunarTowerNebula] = true;
                NoGlobalDrops[NPCID.LunarTowerSolar] = true;
                NoGlobalDrops[NPCID.LunarTowerStardust] = true;
                NoGlobalDrops[NPCID.VortexHornet] = true;
                NoGlobalDrops[NPCID.VortexHornetQueen] = true;
                NoGlobalDrops[NPCID.VortexLarva] = true;
                NoGlobalDrops[NPCID.VortexRifleman] = true;
                NoGlobalDrops[NPCID.StardustCellBig] = true;
                NoGlobalDrops[NPCID.StardustCellSmall] = true;
                NoGlobalDrops[NPCID.StardustJellyfishBig] = true;
                NoGlobalDrops[NPCID.StardustJellyfishSmall] = true;
                NoGlobalDrops[NPCID.StardustSoldier] = true;
                NoGlobalDrops[NPCID.StardustSpiderBig] = true;
                NoGlobalDrops[NPCID.StardustSpiderSmall] = true;
                NoGlobalDrops[NPCID.StardustWormHead] = true;
                NoGlobalDrops[NPCID.SolarCorite] = true;
                NoGlobalDrops[NPCID.SolarCrawltipedeHead] = true;
                NoGlobalDrops[NPCID.SolarDrakomire] = true;
                NoGlobalDrops[NPCID.SolarDrakomireRider] = true;
                NoGlobalDrops[NPCID.SolarFlare] = true;
                NoGlobalDrops[NPCID.SolarGoop] = true;
                NoGlobalDrops[NPCID.SolarSolenian] = true;
                NoGlobalDrops[NPCID.SolarSpearman] = true;
                NoGlobalDrops[NPCID.SolarSroller] = true;
                NoGlobalDrops[NPCID.NebulaBeast] = true;
                NoGlobalDrops[NPCID.NebulaBrain] = true;
                NoGlobalDrops[NPCID.NebulaHeadcrab] = true;
                NoGlobalDrops[NPCID.NebulaSoldier] = true;
                NoGlobalDrops[NPCID.MartianDrone] = true;
                NoGlobalDrops[NPCID.MartianEngineer] = true;
                NoGlobalDrops[NPCID.MartianOfficer] = true;
                NoGlobalDrops[NPCID.MartianProbe] = true;
                NoGlobalDrops[NPCID.MartianSaucer] = true;
                NoGlobalDrops[NPCID.MartianSaucerCore] = true;
                NoGlobalDrops[NPCID.MartianSaucerTurret] = true;
                NoGlobalDrops[NPCID.MartianTurret] = true;
                NoGlobalDrops[NPCID.MartianWalker] = true;
                NoGlobalDrops[NPCID.ForceBubble] = true;
                NoGlobalDrops[NPCID.CultistDragonHead] = true;
                NoGlobalDrops[NPCID.AncientCultistSquidhead] = true;
                NoGlobalDrops[NPCID.AncientDoom] = true;
                NoGlobalDrops[NPCID.AncientLight] = true;
                NoGlobalDrops[NPCID.Creeper] = true;
                NoGlobalDrops[NPCID.Sharkron] = true;
                NoGlobalDrops[NPCID.Sharkron2] = true;
                NoGlobalDrops[NPCID.DetonatingBubble] = true;
                NoGlobalDrops[NPCID.DemonTaxCollector] = true;
                NoGlobalDrops[NPCID.DungeonSpirit] = true;
                NoGlobalDrops[NPCID.DungeonGuardian] = true;
                NoGlobalDrops[NPCID.Slimer] = true;
                NoGlobalDrops[NPCID.Bee] = true;
                NoGlobalDrops[NPCID.BeeSmall] = true;
                NoGlobalDrops[NPCID.CrimsonBunny] = true;
                NoGlobalDrops[NPCID.CrimsonPenguin] = true;
                NoGlobalDrops[NPCID.CrimsonGoldfish] = true;
                NoGlobalDrops[NPCID.CorruptBunny] = true;
                NoGlobalDrops[NPCID.CorruptPenguin] = true;
                NoGlobalDrops[NPCID.CorruptGoldfish] = true;
                NoGlobalDrops[NPCID.GoblinArcher] = true;
                NoGlobalDrops[NPCID.GoblinPeon] = true;
                NoGlobalDrops[NPCID.GoblinScout] = true;
                NoGlobalDrops[NPCID.GoblinSorcerer] = true;
                NoGlobalDrops[NPCID.GoblinSummoner] = true;
                NoGlobalDrops[NPCID.GoblinThief] = true;
                NoGlobalDrops[NPCID.GoblinWarrior] = true;
                NoGlobalDrops[NPCID.BoundGoblin] = true;
                NoGlobalDrops[NPCID.PirateShip] = true;
                NoGlobalDrops[NPCID.PirateShipCannon] = true;
                NoGlobalDrops[NPCID.PirateCaptain] = true;
                NoGlobalDrops[NPCID.PirateCorsair] = true;
                NoGlobalDrops[NPCID.PirateCrossbower] = true;
                NoGlobalDrops[NPCID.PirateDeadeye] = true;
                NoGlobalDrops[NPCID.PirateDeckhand] = true;
                NoGlobalDrops[NPCID.SnowmanGangsta] = true;
                NoGlobalDrops[NPCID.SnowBalla] = true;
                NoGlobalDrops[NPCID.MisterStabby] = true;
                NoGlobalDrops[NPCID.MothronEgg] = true;
                NoGlobalDrops[NPCID.MothronSpawn] = true;
                NoGlobalDrops[NPCID.RayGunner] = true;
                NoGlobalDrops[NPCID.Scutlix] = true;
                NoGlobalDrops[NPCID.ScutlixRider] = true;
                NoGlobalDrops[NPCID.GrayGrunt] = true;
                NoGlobalDrops[NPCID.Scarecrow1] = true;
                NoGlobalDrops[NPCID.Scarecrow2] = true;
                NoGlobalDrops[NPCID.Scarecrow3] = true;
                NoGlobalDrops[NPCID.Scarecrow4] = true;
                NoGlobalDrops[NPCID.Scarecrow5] = true;
                NoGlobalDrops[NPCID.Scarecrow6] = true;
                NoGlobalDrops[NPCID.Scarecrow7] = true;
                NoGlobalDrops[NPCID.Scarecrow8] = true;
                NoGlobalDrops[NPCID.Scarecrow9] = true;
                NoGlobalDrops[NPCID.Scarecrow10] = true;
                NoGlobalDrops[NPCID.Splinterling] = true;
                NoGlobalDrops[NPCID.Hellhound] = true;
                NoGlobalDrops[NPCID.Poltergeist] = true;
                NoGlobalDrops[NPCID.MourningWood] = true;
                NoGlobalDrops[NPCID.Pumpking] = true;
                NoGlobalDrops[NPCID.PumpkingBlade] = true;
                NoGlobalDrops[NPCID.ElfArcher] = true;
                NoGlobalDrops[NPCID.ElfCopter] = true;
                NoGlobalDrops[NPCID.ZombieElf] = true;
                NoGlobalDrops[NPCID.ZombieElfBeard] = true;
                NoGlobalDrops[NPCID.ZombieElfGirl] = true;
                NoGlobalDrops[NPCID.GingerbreadMan] = true;
                NoGlobalDrops[NPCID.Flocko] = true;
                NoGlobalDrops[NPCID.HeadlessHorseman] = true;
                NoGlobalDrops[NPCID.Nutcracker] = true;
                NoGlobalDrops[NPCID.NutcrackerSpinning] = true;
                NoGlobalDrops[NPCID.Yeti] = true;
                NoGlobalDrops[NPCID.Krampus] = true;
                NoGlobalDrops[NPCID.PresentMimic] = true;
                NoGlobalDrops[NPCID.Everscream] = true;
                NoGlobalDrops[NPCID.SantaNK1] = true;
                NoGlobalDrops[NPCID.IceQueen] = true;
                NoGlobalDrops[NPCID.PrimeCannon] = true;
                NoGlobalDrops[NPCID.PrimeLaser] = true;
                NoGlobalDrops[NPCID.PrimeSaw] = true;
                NoGlobalDrops[NPCID.PrimeVice] = true;
                NoGlobalDrops[NPCID.SkeletronHand] = true;
                NoGlobalDrops[NPCID.DD2EterniaCrystal] = true;
                NoGlobalDrops[NPCID.FungiSpore] = true;
                NoGlobalDrops[NPCID.Spore] = true;
                NoGlobalDrops[NPCID.CultistDevote] = true;
                NoGlobalDrops[NPCID.CultistArcherBlue] = true;
                NoGlobalDrops[NPCID.BigMimicHallow] = true;
                NoGlobalDrops[NPCID.BigMimicCorruption] = true;
                NoGlobalDrops[NPCID.BigMimicCrimson] = true;
                NoGlobalDrops[NPCID.TargetDummy] = true;
                NoGlobalDrops[NPCID.DD2LanePortal] = true;
                NoGlobalDrops[NPCID.BartenderUnconscious] = true;
                NoGlobalDrops[NPCID.BoundMechanic] = true;
                NoGlobalDrops[NPCID.BoundWizard] = true;
                NoGlobalDrops[NPCID.EaterofWorldsHead] = true;
                NoGlobalDrops[NPCID.EaterofWorldsBody] = true;
                NoGlobalDrops[NPCID.EaterofWorldsTail] = true;
                NoGlobalDrops[NPCID.DungeonSpirit] = true;
                NoGlobalDrops[ModContent.NPCType<Heckto>()] = true;
                NoGlobalDrops[ModContent.NPCType<Meteor>()] = true;

                UnaffectedByWind = new bool[NPCLoader.NPCCount];

                for (int i = 0; i < NPCLoader.NPCCount; i++)
                {
                    if (NPCID.Sets.BelongsToInvasionOldOnesArmy[i])
                    {
                        UnaffectedByWind[i] = true;
                        continue;
                    }
                    try
                    {
                        var npc = new NPC();
                        npc.SetDefaults(i);
                        if (npc.aiStyle != AIStyles.DemonEyeAI && npc.aiStyle != AIStyles.FlyingAI && npc.aiStyle != AIStyles.SpellAI && npc.aiStyle != AIStyles.EnchantedSwordAI && npc.aiStyle != AIStyles.SpiderAI &&
                            (npc.noGravity || npc.boss))
                        {
                            UnaffectedByWind[i] = true;
                        }
                    }
                    catch (Exception e)
                    {
                        UnaffectedByWind[i] = true;
                        var l = AQMod.GetInstance().Logger;
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

                UnaffectedByWind[NPCID.BigMimicCorruption] = true;
                UnaffectedByWind[NPCID.BigMimicCrimson] = true;
                UnaffectedByWind[NPCID.BigMimicHallow] = true;
                UnaffectedByWind[NPCID.BigMimicJungle] = true;
                UnaffectedByWind[NPCID.MourningWood] = true;
                UnaffectedByWind[NPCID.Everscream] = true;
                UnaffectedByWind[NPCID.SantaNK1] = true;
                UnaffectedByWind[NPCID.CultistArcherBlue] = true;
                UnaffectedByWind[NPCID.CultistDevote] = true;
                UnaffectedByWind[NPCID.TargetDummy] = true;
                UnaffectedByWind[NPCID.Antlion] = true;
                UnaffectedByWind[NPCID.Paladin] = true;
                UnaffectedByWind[NPCID.Yeti] = true;
                UnaffectedByWind[NPCID.Krampus] = true;
                UnaffectedByWind[NPCID.BrainScrambler] = true;
                UnaffectedByWind[NPCID.RayGunner] = true;
                UnaffectedByWind[NPCID.MartianOfficer] = true;
                UnaffectedByWind[NPCID.GrayGrunt] = true;
                UnaffectedByWind[NPCID.MartianEngineer] = true;
                UnaffectedByWind[NPCID.GigaZapper] = true;
                UnaffectedByWind[NPCID.Scutlix] = true;
                UnaffectedByWind[NPCID.ScutlixRider] = true;
                UnaffectedByWind[NPCID.StardustSoldier] = true;
                UnaffectedByWind[NPCID.StardustSpiderBig] = true;
                UnaffectedByWind[NPCID.StardustSpiderSmall] = true;
                UnaffectedByWind[NPCID.SolarDrakomire] = true;
                UnaffectedByWind[NPCID.SolarDrakomireRider] = true;
                UnaffectedByWind[NPCID.SolarSroller] = true;
                UnaffectedByWind[NPCID.SolarSolenian] = true;
                UnaffectedByWind[NPCID.NebulaBeast] = true;
                UnaffectedByWind[NPCID.NebulaSoldier] = true;
                UnaffectedByWind[NPCID.VortexRifleman] = true;
                UnaffectedByWind[NPCID.VortexSoldier] = true;
                UnaffectedByWind[NPCID.VortexLarva] = true;
                UnaffectedByWind[NPCID.VortexHornet] = true;
                UnaffectedByWind[NPCID.VortexHornetQueen] = true;
                UnaffectedByWind[NPCID.Nailhead] = true;
                UnaffectedByWind[NPCID.Eyezor] = true;
                UnaffectedByWind[NPCID.GoblinSummoner] = true;
                UnaffectedByWind[NPCID.SolarSpearman] = true;
                UnaffectedByWind[NPCID.MartianWalker] = true;
                UnaffectedByWind[ModContent.NPCType<NPCs.Monsters.CrabSeason.StriderCrab>()] = true;

                UnaffectedByWind[NPCID.KingSlime] = false;
                UnaffectedByWind[NPCID.EyeofCthulhu] = false;
                UnaffectedByWind[NPCID.BrainofCthulhu] = false;
                UnaffectedByWind[NPCID.EaterofWorldsHead] = false;
                UnaffectedByWind[NPCID.SkeletronHand] = false;
                UnaffectedByWind[NPCID.DevourerHead] = false;
                UnaffectedByWind[NPCID.GiantWormHead] = false;
                UnaffectedByWind[NPCID.Hornet] = false;
                UnaffectedByWind[NPCID.HornetFatty] = false;
                UnaffectedByWind[NPCID.HornetHoney] = false;
                UnaffectedByWind[NPCID.HornetLeafy] = false;
                UnaffectedByWind[NPCID.HornetSpikey] = false;
                UnaffectedByWind[NPCID.HornetStingy] = false;
                UnaffectedByWind[NPCID.ManEater] = false;
                UnaffectedByWind[NPCID.Snatcher] = false;
                UnaffectedByWind[NPCID.EaterofSouls] = false;
                UnaffectedByWind[NPCID.Corruptor] = false;
                UnaffectedByWind[NPCID.Crimera] = false;
                UnaffectedByWind[NPCID.IceElemental] = false;
                UnaffectedByWind[NPCID.AnomuraFungus] = false;
                UnaffectedByWind[NPCID.MushiLadybug] = false;
                UnaffectedByWind[NPCID.Duck2] = false;
                UnaffectedByWind[NPCID.DuckWhite2] = false;
                UnaffectedByWind[NPCID.DetonatingBubble] = false;
                UnaffectedByWind[NPCID.DungeonSpirit] = false;
                UnaffectedByWind[ModContent.NPCType<Heckto>()] = false;
                UnaffectedByWind[ModContent.NPCType<NPCs.Monsters.GlimmerEvent.Starite>()] = false;
                UnaffectedByWind[ModContent.NPCType<TrapImp>()] = false;
                UnaffectedByWind[ModContent.NPCType<Cindera>()] = false;
                UnaffectedByWind[ModContent.NPCType<Meteor>()] = false;
                UnaffectedByWind[ModContent.NPCType<NPCs.Monsters.CrabSeason.ArrowCrab>()] = false;
                UnaffectedByWind[ModContent.NPCType<NPCs.Monsters.CrabSeason.StriderCrab>()] = false;
                UnaffectedByWind[ModContent.NPCType<NPCs.Monsters.CrabSeason.SoliderCrabs>()] = false;
                UnaffectedByWind[ModContent.NPCType<BalloonMerchant>()] = false;
            }

            internal static void UnloadSets()
            {
                DemonSiegeEnemy = null;
                EnemyDungeonSprit = null;
                HecktoplasmDungeonEnemy = null;
                NoSpoilLoot = null;
                NoMapBlip = null;
                NoGlobalDrops = null;
                UnaffectedByWind = null;
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

        internal static Color GreenSlimeColor => new Color(0, 220, 40, 100);
        internal static Color BlueSlimeColor => new Color(0, 80, 255, 100);

        public static bool BossRush { get; private set; }
        public static byte BossRushPlayer { get; private set; }

        private static int _breadsoul = 0;

        public override bool InstancePerEntity => true;

        public bool shimmering;
        /// <summary>
        /// Blue Fire
        /// </summary>
        public bool notFrostburn;
        /// <summary>
        /// When this flag is raised, no wind events should be applied to this NPC
        /// </summary>
        public bool windStruck;
        public bool windStruckOld;
        /// <summary>
        /// A flag for Aphrodite's and Venus' custom lovestruck effects
        /// </summary>
        public bool lovestruckAQ;
        public bool corruptHellfire;
        public bool crimsonHellfire;

        public bool hotDamage;
        public sbyte temperature;

        public override void ResetEffects(NPC npc)
        {
            shimmering = false;
            notFrostburn = false;
            windStruckOld = windStruck;
            windStruck = false;
            lovestruckAQ = false;
            corruptHellfire = false;
            crimsonHellfire = false;
        }

        public override void SetDefaults(NPC npc)
        {
            npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.LovestruckAQ>()] = npc.buffImmune[BuffID.Lovestruck];

            if (npc.buffImmune[BuffID.CursedInferno] || npc.buffImmune[BuffID.ShadowFlame])
            {
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.BlueFire>()] = true;
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.Sparkling>()] = true;
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
            }

            if (npc.type == NPCID.DarkMummy || npc.type == NPCID.DesertDjinn || npc.type == NPCID.DesertLamiaDark)
            {
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
                npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
            }
            else
            {
                switch (npc.type)
                {
                    case NPCID.EaterofSouls:
                    case NPCID.BigEater:
                    case NPCID.LittleEater:
                    case NPCID.EaterofWorldsHead:
                    case NPCID.EaterofWorldsBody:
                    case NPCID.EaterofWorldsTail:
                    case NPCID.DevourerHead:
                    case NPCID.DevourerBody:
                    case NPCID.DevourerTail:
                    case NPCID.Clinger:
                    case NPCID.CorruptBunny:
                    case NPCID.CorruptGoldfish:
                    case NPCID.Corruptor:
                    case NPCID.CorruptPenguin:
                    case NPCID.CorruptSlime:
                    case NPCID.Slimeling:
                    case NPCID.Slimer:
                    case NPCID.Slimer2:
                    case NPCID.BigMimicCorruption:
                    case NPCID.DesertGhoulCorruption:
                    case NPCID.PigronCorruption:
                    case NPCID.SandsharkCorrupt:
                        {
                            npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
                        }
                        break;

                    case NPCID.Crimera:
                    case NPCID.FaceMonster:
                    case NPCID.BloodCrawler:
                    case NPCID.BloodCrawlerWall:
                    case NPCID.Crimslime:
                    case NPCID.Herpling:
                    case NPCID.CrimsonGoldfish:
                    case NPCID.BrainofCthulhu:
                    case NPCID.Creeper:
                    case NPCID.BloodJelly:
                    case NPCID.BloodFeeder:
                    case NPCID.CrimsonAxe:
                    case NPCID.IchorSticker:
                    case NPCID.FloatyGross:
                    case NPCID.DesertGhoulCrimson:
                    case NPCID.PigronCrimson:
                    case NPCID.BigMimicCrimson:
                    case NPCID.CrimsonBunny:
                    case NPCID.CrimsonPenguin:
                        {
                            npc.buffImmune[ModContent.BuffType<Buffs.Debuffs.CrimsonHellfire>()] = true;
                        }
                        break;
                }
            }
        }

        public override void AI(NPC npc)
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

        public bool ShouldApplyWindMechanics(NPC npc)
        {
            return !windStruck && !Sets.UnaffectedByWind[npc.type];
        }

        public void ApplyWindMechanics(NPC npc, Vector2 wind)
        {
            npc.velocity += wind;
            windStruck = true;
        }

        public static bool AreTheSameNPC(int type, int otherType)
        {
            if (type == otherType || Item.NPCtoBanner(type) == Item.NPCtoBanner(otherType)) // same banner or same npc type
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
            }
            return false;
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
            if (notFrostburn)
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

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (shimmering)
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
                    ParticleLayers.AddParticle_PostDrawPlayers(
                        new MonoParticle(dustPos, velocity,
                        new Color(0.9f, Main.rand.NextFloat(0.6f, 0.9f), Main.rand.NextFloat(0.4f, 1f), 0f), Main.rand.NextFloat(0.3f, 1f)));

                    ParticleLayers.AddParticle_PostDrawPlayers(
                        new MonoParticle(dustPos, velocity,
                        new Color(0.9f, Main.rand.NextFloat(0.6f, 0.9f), Main.rand.NextFloat(0.4f, 1f), 0f) * 0.2f, 0.5f));

                    if (Main.rand.NextBool(30))
                    {
                        dustPos = npc.position + new Vector2(Main.rand.Next(npc.width + 4) - 2f, Main.rand.Next(npc.height + 4) - 2f);
                        normal = Vector2.Normalize(dustPos - center);
                        velocity = normal * Main.rand.NextFloat(size / 12f, size / 6f);
                        velocity += -npc.velocity * 0.3f;
                        velocity *= 0.01f;

                        var sparkleClr = new Color(0.5f, Main.rand.NextFloat(0.1f, 0.5f), Main.rand.NextFloat(0.1f, 0.55f), 0f);
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new BrightSparkle(dustPos, velocity,
                            sparkleClr, 1f));
                    }
                }
                Lighting.AddLight(npc.Center, 0.25f, 0.25f, 0.25f);
            }
            if (notFrostburn)
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
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new EmberParticle(dustPos, new Vector2((npc.velocity.X + Main.rand.NextFloat(-3f, 3f)) * 0.3f, ((npc.velocity.Y + Main.rand.NextFloat(-3f, 3f)) * 0.4f).Abs() - 2f),
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.2f, 1.2f)));
                    }
                }
                Lighting.AddLight(npc.Center, 0.4f, 0.4f, 1f);
            }
            if (corruptHellfire || crimsonHellfire)
            {
                Color fireColor = Buffs.Debuffs.CorruptionHellfire.FireColor;
                if (corruptHellfire && crimsonHellfire)
                {
                    fireColor = Color.Lerp(fireColor, Buffs.Debuffs.CrimsonHellfire.FireColor, 0.5f);
                }
                else if (crimsonHellfire)
                {
                    fireColor = Buffs.Debuffs.CrimsonHellfire.FireColor;
                }
                if (Main.netMode != NetmodeID.Server && AQGraphics.GameWorldActive)
                {
                    var pos = npc.position - new Vector2(2f, 2f);
                    var rect = new Rectangle((int)pos.X, (int)pos.Y, npc.width + 4, npc.height + 4);
                    for (int i = 0; i < 4; i++)
                    {
                        var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                        var velocity = new Vector2((npc.velocity.X + Main.rand.NextFloat(-3f, 3f)) * 0.3f, ((npc.velocity.Y + Main.rand.NextFloat(-3f, 3f)) * 0.4f).Abs() - 2f);
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new EmberParticle(dustPos, velocity,
                            fireColor, Main.rand.NextFloat(0.8f, 1.1f)));
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new EmberParticle(dustPos, velocity,
                            fireColor * 0.2f, 1.5f));
                    }
                }
                Lighting.AddLight(npc.Center, fireColor.ToVector3());
            }
        }

        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            if (npc.life < 0 && npc.HasValidTarget && Main.myPlayer == npc.target && npc.CanBeChasedBy(Main.player[npc.target]))
            {
                if (shimmering)
                {
                    int amount = (int)(100 * AQConfigClient.c_EffectIntensity);
                    if (AQConfigClient.c_EffectQuality < 1f)
                        amount = (int)(amount * AQConfigClient.c_EffectQuality);
                    if (AQConfigClient.c_Screenshakes)
                        ScreenShakeManager.AddShake(new BasicScreenShake(12, 6));
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
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.9f, Main.rand.NextFloat(0.7f, 0.9f), Main.rand.NextFloat(0.4f, 1f), 0f), Main.rand.NextFloat(0.8f, 1.1f)));
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.9f, Main.rand.NextFloat(0.7f, 0.9f), Main.rand.NextFloat(0.4f, 1f), 0f) * 0.2f, 1.5f));
                        if (Main.rand.NextBool(14))
                        {
                            var sparkleClr = new Color(0.9f, Main.rand.NextFloat(0.8f, 0.9f), Main.rand.NextFloat(0.4f, 1f), 0f);
                            ParticleLayers.AddParticle_PostDrawPlayers(
                                new SparkleParticle(dustPos, velocity,
                                sparkleClr, 1.5f));
                            ParticleLayers.AddParticle_PostDrawPlayers(
                                new SparkleParticle(dustPos, velocity,
                                sparkleClr * 0.5f, 1f)
                                { rotation = MathHelper.PiOver4 });
                        }
                    }
                }
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            ApplyDamageEffects(ref damage);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if ((projectile.type == ProjectileID.MolotovFire || projectile.type == ProjectileID.MolotovFire2 || projectile.type == ProjectileID.MolotovFire3) &&
                npc.type == NPCID.TheDestroyerBody)
            {

            }
            if (npc.townNPC)
            {
                if (projectile.type == ModContent.ProjectileType<OmegaRay>())
                    damage = (int)(damage * 0.1f);
            }
            ApplyDamageEffects(ref damage);
        }

        private void ApplyDamageEffects(ref int damage)
        {
            if (lovestruckAQ)
                damage += (int)(damage * 0.1f);
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (lovestruckAQ)
                damage -= (int)(damage * 0.1f);
        }

        public override bool PreAI(NPC npc)
        {
            if (MoonlightWallHelper.Active) // in case the NPC before this one broke and skipped PostAI, if there's a next NPC then it would hopefully fix it
                MoonlightWallHelper.End();
            if (MoonlightWallHelper.BehindMoonlightWall(npc.Center))
                MoonlightWallHelper.Begin();
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
            switch (npc.type)
            {
                case NPCID.Ghost:
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer && Main.LocalPlayer.GetModPlayer<AQPlayer>().ghostAmulet)
                        {
                            npc.life = -1;
                            npc.HitEffect();
                            npc.active = false;
                            return false;
                        }
                    }
                    break;

                case NPCID.VoodooDemon:
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer && Main.LocalPlayer.GetModPlayer<AQPlayer>().voodooAmulet)
                        {
                            npc.life = -1;
                            npc.HitEffect();
                            npc.active = false;
                            return false;
                        }
                    }
                    break;

                case NPCID.WyvernHead:
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer && Main.LocalPlayer.GetModPlayer<AQPlayer>().wyvernAmulet)
                        {
                            npc.life = -1;
                            npc.HitEffect();
                            npc.active = false;
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }

        public override void PostAI(NPC npc)
        {

            if (MoonlightWallHelper.Active)
                MoonlightWallHelper.End();
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

        private void EncoreKill(NPC npc)
        {
            if (!npc.boss || npc.type == NPCID.MartianSaucerCore || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
                return;
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                var player = Main.player[k];
                if (!player.active || player.dead)
                {
                    continue;
                }
                var aQPlayer = player.GetModPlayer<AQPlayer>();
                var bossRushPlayer = player.GetModPlayer<BossEncorePlayer>();
                if (!aQPlayer.bossrush)
                {
                    bossRushPlayer.CurrentEncoreKillCount[npc.type] = 0;
                    return;
                }
                bossRushPlayer.CurrentEncoreKillCount[npc.type]++;
                if (bossRushPlayer.CurrentEncoreKillCount[npc.type] > bossRushPlayer.EncoreBossKillCountRecord[npc.type])
                    bossRushPlayer.EncoreBossKillCountRecord[npc.type] = bossRushPlayer.CurrentEncoreKillCount[npc.type];
                if (Main.netMode == NetmodeID.Server)
                {
                    NetHelper.NetCombatText(player.getRect(), Main.mouseColor, bossRushPlayer.CurrentEncoreKillCount[npc.type]);
                }
                else
                {
                    CombatText.NewText(player.getRect(), Main.mouseColor, bossRushPlayer.CurrentEncoreKillCount[npc.type], true);
                }
                float x = player.position.X + player.width / 2f;
                if (npc.type == NPCID.TheDestroyer && npc.lifeMax <= 0)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].type == NPCID.TheDestroyerBody || Main.npc[i].type == NPCID.TheDestroyerTail)
                        {
                            Main.npc[i].lifeMax = -1;
                            Main.npc[i].HitEffect();
                            Main.npc[i].active = false;
                        }
                    }
                }
                int n = NPC.NewNPC((int)x, (int)player.position.Y - 600, npc.type);
                NPC boss = Main.npc[n];
                boss.netUpdate = true;
                float yOff = boss.height * 2f;
                float healthScale = bossRushScale(npc.lifeMax);
                float damageScale = MathHelper.Clamp(healthScale, 1.01f, 1.15f);
                boss.lifeMax = (int)(npc.lifeMax * healthScale);
                boss.life = boss.lifeMax;
                boss.defDamage = (int)(npc.defDamage * damageScale);
                boss.damage = boss.defDamage;
                var spawnPosition = new Vector2(x, player.position.Y - yOff);
                boss.Center = spawnPosition;
                boss.target = BossRushPlayer;
                if (npc.type == NPCID.SkeletronHead)
                {
                    boss.ai[0] = 1f;
                    boss.velocity.X = Main.rand.NextFloat(-15f, 15f);
                    int x1 = (int)spawnPosition.X;
                    int y1 = (int)spawnPosition.Y;
                    for (int i = 0; i < 4; i++)
                    {
                        NPC arm = Main.npc[NPC.NewNPC(x1, y1, NPCID.SkeletronHand, boss.whoAmI)];
                        arm.lifeMax = (int)(arm.lifeMax * healthScale);
                        arm.life = arm.lifeMax;
                        arm.Center = spawnPosition;
                        arm.ai[0] = i % 2 == 0 ? -1f : 1f;
                        arm.ai[1] = n;
                        arm.target = BossRushPlayer;
                        arm.netUpdate = true;
                        arm.defDamage = (int)(arm.defDamage * healthScale);
                        arm.damage = arm.defDamage;
                    }
                }
                else if (npc.type == NPCID.SkeletronPrime)
                {
                    boss.ai[0] = 1f;
                    boss.velocity.X = Main.rand.NextFloat(-15f, 15f);
                    int x1 = (int)spawnPosition.X;
                    int y1 = (int)spawnPosition.Y;
                    for (int i = 0; i < 4; i++)
                    {
                        NPC arm = Main.npc[NPC.NewNPC(x1, y1, NPCID.PrimeCannon + i, boss.whoAmI)];
                        arm.lifeMax = (int)(arm.lifeMax * healthScale);
                        arm.life = arm.lifeMax;
                        arm.Center = spawnPosition;
                        arm.ai[0] = i % 2 == 0 ? -1f : 1f;
                        arm.ai[1] = n;
                        arm.target = BossRushPlayer;
                        arm.netUpdate = true;
                        arm.defDamage = (int)(arm.defDamage * healthScale);
                        arm.damage = arm.defDamage;
                        if (i > 1)
                            arm.ai[3] = 150f;
                    }
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetHelper.SyncEncoreData(bossRushPlayer);
                }
            }
        }

        private void UpdateBreadsoul(NPC npc)
        {
            _breadsoul = -1;
            float breadsoulDistance = 2000f;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var plr = Main.player[i];
                if (plr.active && !plr.dead)
                {
                    var aQPlr = plr.GetModPlayer<AQPlayer>();
                    float distance = Vector2.Distance(plr.Center, npc.Center);
                    if (aQPlr.breadsoul && distance < breadsoulDistance)
                    {
                        _breadsoul = i;
                        breadsoulDistance = distance;
                    }
                }
            }
            if (_breadsoul == -1)
                return;
            NPCLoader.blockLoot.Add(ItemID.Heart);
            if (npc.boss)
            {
                BreadsoulHealing.SpawnCluster(Main.player[_breadsoul], npc.Center, npc.Size.Length() / 2f, Main.rand.Next(10, 18), Main.rand.Next(120, 180));
            }
            else
            {
                var breadsoulCollector = Main.player[_breadsoul].Center;
                int lowestPercentPlayer = _breadsoul;
                float lifePercent = Main.player[_breadsoul].statLife / (float)Main.player[_breadsoul].statLifeMax2;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (i == _breadsoul || Main.player[i].dead || !Main.player[i].active)
                        continue;
                    if (Vector2.Distance(breadsoulCollector, Main.player[i].Center) < 2000f)
                    {
                        float otherLifePercent = Main.player[i].statLife / (float)Main.player[i].statLifeMax2;
                        if (otherLifePercent < lifePercent)
                        {
                            lowestPercentPlayer = i;
                            lifePercent = otherLifePercent;
                        }
                    }
                }
                if (lifePercent > 0.9f)
                    return;
                int chance = (int)(lifePercent * 40) + Main.player[lowestPercentPlayer].statDefense / 3;
                if (chance <= 1 || Main.rand.NextBool(chance))
                    BreadsoulHealing.SpawnCluster(Main.player[_breadsoul], npc.Center, Main.rand.Next(3, 6), Main.rand.Next(25, 35));
            }
        }

        private void ManageDreadsoul(NPC npc)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var plr = Main.player[i];
                if (plr.active && !plr.dead)
                {
                    var aQPlr = plr.GetModPlayer<AQPlayer>();
                    float distance = Vector2.Distance(plr.Center, npc.Center);
                    int dreadsoulAttack = ModContent.ProjectileType<DreadsoulAttack>();
                    if (aQPlr.dreadsoul && distance < 2000f && plr.ownedProjectileCounts[dreadsoulAttack] < 36)
                    {
                        var center = npc.Center;
                        float rot = MathHelper.TwoPi / 6f;
                        int damage = (int)(plr.GetWeaponDamage(plr.HeldItem) * 0.65f);
                        float kb = plr.GetWeaponKnockback(plr.HeldItem, 1f) / 6f;
                        float size = (float)Math.Sqrt(npc.width * npc.height);
                        for (int j = 0; j < 6; j++)
                        {
                            var dir = new Vector2(0f, -1f).RotatedBy(rot * j);
                            Projectile.NewProjectile(center + dir * (size + 2f), dir * 2f, dreadsoulAttack, damage, kb, i, 0f, npc.type);
                        }
                        plr.ownedProjectileCounts[dreadsoulAttack] += 6;
                        break;
                    }
                }
            }
        }

        public override bool PreNPCLoot(NPC npc)
        {
            if (NPCLootLooper.CurrentNPCLootLoop != 0)
            {
                NPCLoader.blockLoot.Add(ItemID.Heart);
            }
            else
            {
                UpdateBreadsoul(npc);
                if (npc.whoAmI == HuntSystem.TargetNPC)
                {
                    if (HuntSystem.Hunt != null)
                        HuntSystem.Hunt.RemoveHunt();
                    HuntSystem.RandomizeHunt(null);
                    AQMod.BroadcastMessage("Mods.AQMod.Common.RobsterNPCDeath", Robster.RobsterBroadcastMessageColor);
                    AQMod.BroadcastMessage("Mods.AQMod.Common.RobsterNPCDeath2", Robster.RobsterBroadcastMessageColor);
                }
            }
            switch (npc.type)
            {
                case NPCID.BlueJellyfish:
                    {
                        if (ModContent.GetInstance<AQConfigServer>().removeJellyfishNecklace)
                        {
                            NPCLoader.blockLoot.Add(ItemID.JellyfishNecklace);
                        }
                    }
                    break;
            }
            return true;
        }

        public override void NPCLoot(NPC npc)
        {
            if (npc.SpawnedFromStatue || NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type])
                return;
            byte p = Player.FindClosest(npc.position, npc.width, npc.height);
            var plr = Main.player[p];
            var aQPlayer = plr.GetModPlayer<AQPlayer>();
            if (NPCLootLooper.CurrentNPCLootLoop == 0)
            {
                ManageDreadsoul(npc);
                EncoreKill(npc);
            }
            if (npc.townNPC && npc.position.Y > (Main.maxTilesY - 200) * 16f) // does this for any town NPC because why not?
            {
                var check = new Rectangle((int)npc.position.X / 16, (int)npc.position.Y / 16, 2, 3);
                bool spawnedItem = false;
                for (int i = check.X; i <= check.X + check.Width; i++)
                {
                    for (int j = check.Y; j <= check.Y + check.Height; j++)
                    {
                        if (Framing.GetTileSafely(i, j).liquid > 0 && Main.tile[i, j].lava())
                        {
                            Item.NewItem(npc.getRect(), ModContent.ItemType<IWillBeBack>());
                            spawnedItem = true;
                            break;
                        }
                    }
                    if (spawnedItem)
                        break;
                }
            }
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (npc.type == NPCID.Ghost)
                {
                    if (!aQPlayer.ghostAmuletHeld && Main.rand.NextBool(15))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<GhostAmulet>());
                }
                else if (npc.type == NPCID.VoodooDemon)
                {
                    if (!aQPlayer.voodooAmuletHeld && (Main.LocalPlayer.killGuide || Main.LocalPlayer.HasItem(ItemID.GuideVoodooDoll)) && Main.rand.NextBool(3))
                    {
                        int itemType = ModContent.ItemType<VoodooAmulet>();
                        if (!AQItem.ItemOnGroundAlready(itemType))
                            Item.NewItem(npc.getRect(), itemType);
                    }
                }
                else if (npc.type == NPCID.WyvernHead)
                {
                    if (!aQPlayer.wyvernAmuletHeld && plr.wingsLogic > 0 && Main.rand.NextBool(3))
                    {
                        int itemType = ModContent.ItemType<WyvernAmulet>();
                        if (!AQItem.ItemOnGroundAlready(itemType))
                            Item.NewItem(npc.getRect(), itemType);
                    }
                }
            }
            if (!Sets.NoGlobalDrops[npc.type] && !npc.boss && npc.lifeMax > 5 && !npc.friendly && !npc.townNPC)
            {
                if (Main.hardMode && npc.position.Y > Main.rockLayer * 16.0 && npc.value > 0f)
                {
                    if (aQPlayer.altEvilDrops && Main.rand.NextBool(5))
                    {
                        if (plr.ZoneCorrupt || plr.ZoneCrimson)
                            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofLight);
                        if (plr.ZoneHoly)
                            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofNight);
                    }
                }
                var tile = Framing.GetTileSafely(Main.player[p].Center.ToTileCoordinates());
                if (!Main.wallHouse[tile.wall])
                {
                    if (Main.player[p].ZoneJungle && tile.wall != TileID.LihzahrdBrick)
                    {
                        if (npc.lifeMax > (Main.expertMode ? Main.hardMode ? 150 : 80 : 30))
                        {
                            int chance = 14;
                            if (npc.lifeMax + npc.defDefense > 350 && npc.type != NPCID.MossHornet) // defDefense is the defense of the NPC when it spawns
                                chance /= 2;
                            if (Main.rand.NextBool(chance))
                                Item.NewItem(npc.getRect(), ModContent.ItemType<OrganicEnergy>());
                        }
                    }
                }
            }
            if (npc.type >= Main.maxNPCTypes)
                return;
            switch (npc.type)
            {
                case NPCID.BlueJellyfish:
                case NPCID.GreenJellyfish:
                    {
                        if (Main.rand.NextBool(15))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<ShockCollar>());
                    }
                    break;

                case NPCID.SeekerHead:
                    {
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<SpicyEel>());
                    }
                    break;

                case NPCID.DiggerHead:
                    {
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<SpicyEel>());
                    }
                    break;

                case NPCID.RaggedCaster:
                case NPCID.RaggedCasterOpenCoat:
                    {
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<GrapePhanta>());
                    }
                    break;

                case NPCID.RustyArmoredBonesAxe:
                case NPCID.RustyArmoredBonesFlail:
                case NPCID.RustyArmoredBonesSword:
                case NPCID.RustyArmoredBonesSwordNoArmor:
                    {
                        if (Main.rand.NextBool(40))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<GrapePhanta>());
                    }
                    break;

                case NPCID.AngryBones:
                    {
                        if (Main.rand.NextBool(80))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<DungeonMap>());
                    }
                    break;

                case NPCID.DarkCaster:
                    {
                        if (Main.rand.NextBool(50))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<DungeonMap>());
                    }
                    break;

                case NPCID.CursedSkull:
                    {
                        if (Main.rand.NextBool(30))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<DungeonMap>());
                    }
                    break;

                case NPCID.Lihzahrd:
                case NPCID.LihzahrdCrawler:
                    {
                        if (Main.rand.NextBool(50))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<LihzahrdMap>());
                    }
                    break;

                case NPCID.FlyingSnake:
                    {
                        if (Main.rand.NextBool(50))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<LihzahrdMap>());
                    }
                    break;

                case NPCID.UndeadViking:
                    {
                        if (Main.rand.NextBool(6))
                            Item.NewItem(npc.getRect(), ModContent.ItemType<CrystalDagger>());
                    }
                    break;

                case NPCID.Crab:
                    {
                        if (Main.rand.NextBool(10))
                        {
                            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Weapons.Melee.VineSword>());
                        }
                        if (CrabSeason.Active)
                        {
                            if (Main.moonPhase % 2 == 0 && !aQPlayer.altEvilDrops || Main.moonPhase % 2 == 1 && aQPlayer.altEvilDrops)
                                Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>());
                        }
                        else
                        {
                            bool hasItem = Main.player[p].HasItem(ModContent.ItemType<CrabClock>());
                            if (Main.rand.NextBool(hasItem ? 10 : 4))
                                Item.NewItem(npc.getRect(), ModContent.ItemType<CrabClock>());
                            if (Main.rand.NextBool())
                                Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>());
                        }
                    }
                    break;

                case NPCID.DarkMummy:
                    {
                        if (aQPlayer.altEvilDrops && Main.rand.NextBool(10))
                            Item.NewItem(npc.getRect(), ItemID.LightShard);
                    }
                    break;

                case NPCID.LightMummy:
                    {
                        if (aQPlayer.altEvilDrops && Main.rand.NextBool(10))
                            Item.NewItem(npc.getRect(), ItemID.DarkShard);
                    }
                    break;

                case NPCID.DungeonSpirit:
                    if (Main.rand.NextBool(45))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<Breadsoul>());
                    break;

                case NPCID.Necromancer:
                    if (Main.rand.NextBool(30))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<Breadsoul>());
                    break;

                case NPCID.DiabolistRed:
                case NPCID.DiabolistWhite:
                    if (Main.rand.NextBool(30))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<Dreadsoul>());
                    break;

                case NPCID.GingerbreadMan:
                    if (Main.rand.NextBool(1000))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<Baguette>());
                    break;

                case NPCID.Mothron:
                    if (NPC.downedAncientCultist && (npc.playerInteraction[p] && npc.GetGlobalNPC<NoHitManager>().hitPlayer[p] || Main.rand.NextBool(10)))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<MothmanMask>());
                    break;

                case NPCID.Golem:
                    {
                        if (Main.moonPhase == 0)
                            Item.NewItem(npc.getRect(), ModContent.ItemType<RustyKnife>());
                    }
                    break;
            }
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
    }
}