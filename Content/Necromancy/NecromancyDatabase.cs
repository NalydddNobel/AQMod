using Aequus.Common;
using Aequus.Common.Utilities;
using Aequus.Content.CrossMod;
using Aequus.Content.Necromancy.Aggression;
using Aequus.Items.Weapons.Summon.Necro;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Necromancy
{
    public class NecromancyDatabase : LoadableType, IAddRecipes, IModCallable
    {
        public static List<int> NecromancyDebuffs { get; private set; }
        public static Dictionary<int, GhostInfo> NPCs { get; private set; }
        public static HashSet<string> AutogeneratorIgnoreMods { get; private set; }

        public override void Load()
        {
            NecromancyDebuffs = new List<int>();
            AggressionType.LoadAggressions();
            PopulateEnemyStats();
            AutogeneratorIgnoreMods = new HashSet<string>() { "Aequus", };
        }
        private static void PopulateEnemyStats()
        {
            NPCs = new Dictionary<int, GhostInfo>()
            {
                [NPCID.GiantWormHead] = GhostInfo.One,
                [NPCID.DevourerHead] = GhostInfo.One,
                [NPCID.TombCrawlerHead] = GhostInfo.One,
                [NPCID.BlueSlime] = GhostInfo.One,
                [NPCID.ZombieRaincoat] = GhostInfo.One,
                [NPCID.ZombieEskimo] = GhostInfo.One,
                [NPCID.MaggotZombie] = GhostInfo.One,
                [NPCID.BloodZombie] = GhostInfo.One,
                [NPCID.UmbrellaSlime] = GhostInfo.One,
                [NPCID.CaveBat] = GhostInfo.One,
                [NPCID.SporeBat] = GhostInfo.One,
                [NPCID.Bee] = GhostInfo.One,
                [NPCID.BeeSmall] = GhostInfo.One,
                [NPCID.BlueJellyfish] = GhostInfo.One,
                [NPCID.PinkJellyfish] = GhostInfo.One,
                [NPCID.Crab] = GhostInfo.One,
                [NPCID.IceSlime] = GhostInfo.One,
                [NPCID.LacBeetle] = GhostInfo.One,
                [NPCID.CochinealBeetle] = GhostInfo.One,
                [NPCID.CyanBeetle] = GhostInfo.One,
                [NPCID.Piranha] = GhostInfo.One,
                [NPCID.Vulture] = GhostInfo.One,
                [NPCID.Raven] = GhostInfo.One,
                [NPCID.SnowFlinx] = GhostInfo.One,
                [NPCID.Antlion] = GhostInfo.One,
                [NPCID.Squid] = GhostInfo.One,
                [NPCID.Skeleton] = GhostInfo.One,
                [NPCID.SkeletonAlien] = GhostInfo.One,
                [NPCID.SkeletonAstonaut] = GhostInfo.One,
                [NPCID.SkeletonTopHat] = GhostInfo.One,
                [NPCID.HeadacheSkeleton] = GhostInfo.One,
                [NPCID.MisassembledSkeleton] = GhostInfo.One,
                [NPCID.PantlessSkeleton] = GhostInfo.One,
                [NPCID.SporeSkeleton] = GhostInfo.One,
                [NPCID.SlimeSpiked] = GhostInfo.One,
                [NPCID.Crawdad] = GhostInfo.One,
                [NPCID.Crawdad2] = GhostInfo.One,
                [NPCID.Salamander] = GhostInfo.One,
                [NPCID.Salamander2] = GhostInfo.One,
                [NPCID.Salamander3] = GhostInfo.One,
                [NPCID.Salamander4] = GhostInfo.One,
                [NPCID.Salamander5] = GhostInfo.One,
                [NPCID.Salamander6] = GhostInfo.One,
                [NPCID.Salamander7] = GhostInfo.One,
                [NPCID.Salamander8] = GhostInfo.One,
                [NPCID.Salamander9] = GhostInfo.One,
                [NPCID.IceBat] = GhostInfo.One,
                [NPCID.SeaSnail] = GhostInfo.One,
                [NPCID.GoblinScout] = GhostInfo.One,
                [NPCID.GraniteGolem] = GhostInfo.One,
                [NPCID.FaceMonster] = GhostInfo.One,
                [NPCID.FlyingAntlion] = GhostInfo.One,
                [NPCID.WalkingAntlion] = GhostInfo.One,
                [NPCID.LarvaeAntlion] = GhostInfo.One,
                [NPCID.GreekSkeleton] = GhostInfo.One,
                [NPCID.JungleBat] = GhostInfo.One,
                [NPCID.UndeadViking] = GhostInfo.One,
                [NPCID.WallCreeper] = GhostInfo.One,
                [NPCID.WallCreeperWall] = GhostInfo.One,
                [NPCID.BloodCrawler] = GhostInfo.One,
                [NPCID.BloodCrawlerWall] = GhostInfo.One,
                [NPCID.Drippler] = GhostInfo.One,
                [NPCID.FlyingFish] = GhostInfo.One,
                [NPCID.Tumbleweed] = GhostInfo.One,
                [NPCID.GoblinArcher] = GhostInfo.One.WithAggro(AggressionType.GoblinArmy),
                [NPCID.GoblinPeon] = GhostInfo.One.WithAggro(AggressionType.GoblinArmy),
                [NPCID.GoblinThief] = GhostInfo.One.WithAggro(AggressionType.GoblinArmy),
                [NPCID.GoblinWarrior] = GhostInfo.One.WithAggro(AggressionType.GoblinArmy),
                [NPCID.ServantofCthulhu] = GhostInfo.One,
                [NPCID.SlimeRibbonRed] = GhostInfo.One,
                [NPCID.SlimeRibbonGreen] = GhostInfo.One,
                [NPCID.SlimeRibbonWhite] = GhostInfo.One,
                [NPCID.SlimeRibbonYellow] = GhostInfo.One,
                [NPCID.SlimeMasked] = GhostInfo.One,

                [NPCID.BoneSerpentHead] = GhostInfo.Two,
                [NPCID.CrimsonPenguin] = GhostInfo.Two,
                [NPCID.CorruptPenguin] = GhostInfo.Two,
                [NPCID.Crimera] = GhostInfo.Two,
                [NPCID.EaterofSouls] = GhostInfo.Two,
                [NPCID.GiantFlyingAntlion] = GhostInfo.Two,
                [NPCID.GiantWalkingAntlion] = GhostInfo.Two,
                [NPCID.CursedSkull] = GhostInfo.Two,
                [NPCID.Demon] = GhostInfo.Two,
                [NPCID.DungeonSlime] = GhostInfo.Two,
                [NPCID.GiantShelly] = GhostInfo.Two,
                [NPCID.GiantShelly2] = GhostInfo.Two,
                [NPCID.GraniteFlyer] = GhostInfo.Two,
                [NPCID.Harpy] = GhostInfo.Two,
                [NPCID.Hellbat] = GhostInfo.Two,
                [NPCID.MeteorHead] = GhostInfo.Two,
                [NPCID.Shark] = GhostInfo.Two,
                [NPCID.SpikedIceSlime] = GhostInfo.Two,
                [NPCID.SpikedJungleSlime] = GhostInfo.Two,
                [NPCID.UndeadMiner] = GhostInfo.Two,
                [NPCID.VoodooDemon] = GhostInfo.Two,
                [NPCID.TheGroom] = GhostInfo.Two,
                [NPCID.TheBride] = GhostInfo.Two,
                [NPCID.ZombieMerman] = GhostInfo.Two,
                [NPCID.EyeballFlyingFish] = GhostInfo.Two,
                [NPCID.MisterStabby] = GhostInfo.Two,
                [NPCID.SnowBalla] = GhostInfo.Two,
                [NPCID.SnowmanGangsta] = GhostInfo.Two,
                [NPCID.Parrot] = GhostInfo.Two,
                [NPCID.QueenSlimeMinionPink] = GhostInfo.Two,
                [NPCID.QueenSlimeMinionPurple] = GhostInfo.Two,
                [NPCID.QueenSlimeMinionBlue] = GhostInfo.Two,
                [NPCID.TheHungryII] = GhostInfo.Two,
                [NPCID.Probe] = GhostInfo.Two,

                [NPCID.DuneSplicerHead] = GhostInfo.Three,
                [NPCID.DiggerHead] = GhostInfo.Three,
                [NPCID.SwampThing] = GhostInfo.Three,
                [NPCID.Unicorn] = GhostInfo.Three,
                [NPCID.AnomuraFungus] = GhostInfo.Three,
                [NPCID.DoctorBones] = GhostInfo.Three,
                [NPCID.FungiBulb] = GhostInfo.Three,
                [NPCID.Ghost] = GhostInfo.Three,
                [NPCID.MushiLadybug] = GhostInfo.Three,
                [NPCID.Nymph] = GhostInfo.Three,
                [NPCID.ZombieMushroom] = GhostInfo.Three,
                [NPCID.ZombieMushroomHat] = GhostInfo.Three,
                [NPCID.AnglerFish] = GhostInfo.Three,
                [NPCID.Arapaima] = GhostInfo.Three,
                [NPCID.ArmoredSkeleton] = GhostInfo.Three,
                [NPCID.ArmoredViking] = GhostInfo.Three,
                [NPCID.DesertBeast] = GhostInfo.Three,
                [NPCID.BlackRecluse] = GhostInfo.Three,
                [NPCID.BlackRecluseWall] = GhostInfo.Three,
                [NPCID.BloodFeeder] = GhostInfo.Three,
                [NPCID.BloodJelly] = GhostInfo.Three,
                [NPCID.Mummy] = GhostInfo.Three,
                [NPCID.BloodMummy] = GhostInfo.Three,
                [NPCID.DarkMummy] = GhostInfo.Three,
                [NPCID.LightMummy] = GhostInfo.Three,
                [NPCID.ChaosElemental] = GhostInfo.Three,
                [NPCID.CorruptSlime] = GhostInfo.Three,
                [NPCID.Corruptor] = GhostInfo.Three,
                [NPCID.Crimslime] = GhostInfo.Three,
                [NPCID.Slimeling] = GhostInfo.Three,
                [NPCID.CrimsonAxe] = GhostInfo.Three,
                [NPCID.CursedHammer] = GhostInfo.Three,
                [NPCID.EnchantedSword] = GhostInfo.Three,
                [NPCID.Derpling] = GhostInfo.Three,
                [NPCID.DesertDjinn] = GhostInfo.Three,
                [NPCID.DesertGhoul] = GhostInfo.Three,
                [NPCID.DesertGhoulCorruption] = GhostInfo.Three,
                [NPCID.DesertGhoulCrimson] = GhostInfo.Three,
                [NPCID.DesertGhoulHallow] = GhostInfo.Three,
                [NPCID.FungoFish] = GhostInfo.Three,
                [NPCID.Gastropod] = GhostInfo.Three,
                [NPCID.GiantBat] = GhostInfo.Three,
                [NPCID.GiantFungiBulb] = GhostInfo.Three,
                [NPCID.GreenJellyfish] = GhostInfo.Three,
                [NPCID.Herpling] = GhostInfo.Three,
                [NPCID.HoppinJack] = GhostInfo.Three,
                [NPCID.IceElemental] = GhostInfo.Three,
                [NPCID.IceMimic] = GhostInfo.Three,
                [NPCID.Mimic] = GhostInfo.Three,
                [NPCID.IceTortoise] = GhostInfo.Three,
                [NPCID.IchorSticker] = GhostInfo.Three,
                [NPCID.IcyMerman] = GhostInfo.Three,
                [NPCID.IlluminantBat] = GhostInfo.Three,
                [NPCID.IlluminantSlime] = GhostInfo.Three,
                [NPCID.DesertLamiaDark] = GhostInfo.Three,
                [NPCID.DesertLamiaLight] = GhostInfo.Three,
                [NPCID.Lavabat] = GhostInfo.Three,
                [NPCID.PigronCorruption] = GhostInfo.Three,
                [NPCID.PigronCrimson] = GhostInfo.Three,
                [NPCID.PigronHallow] = GhostInfo.Three,
                [NPCID.Pixie] = GhostInfo.Three,
                [NPCID.PossessedArmor] = GhostInfo.Three,
                [NPCID.DesertScorpionWalk] = GhostInfo.Three,
                [NPCID.DesertScorpionWall] = GhostInfo.Three,
                [NPCID.ToxicSludge] = GhostInfo.Three,
                [NPCID.Werewolf] = GhostInfo.Three,
                [NPCID.Wolf] = GhostInfo.Three,
                [NPCID.SkeletonArcher] = GhostInfo.Three,
                [NPCID.WanderingEye] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.Clown] = GhostInfo.Three,
                [NPCID.ChatteringTeethBomb] = GhostInfo.Three,
                [NPCID.SandShark] = GhostInfo.Three,
                [NPCID.SandsharkCorrupt] = GhostInfo.Three,
                [NPCID.SandsharkCrimson] = GhostInfo.Three,
                [NPCID.SandsharkHallow] = GhostInfo.Three,
                [NPCID.BloodSquid] = GhostInfo.Three,
                [NPCID.AngryNimbus] = GhostInfo.Three,
                [NPCID.RainbowSlime] = GhostInfo.Three,
                [NPCID.GoblinSummoner] = GhostInfo.Three.WithAggro(AggressionType.GoblinArmy),
                [NPCID.PirateCorsair] = GhostInfo.Three.WithAggro(AggressionType.PirateInvasion),
                [NPCID.PirateCrossbower] = GhostInfo.Three.WithAggro(AggressionType.PirateInvasion),
                [NPCID.PirateDeadeye] = GhostInfo.Three.WithAggro(AggressionType.PirateInvasion),
                [NPCID.PirateDeckhand] = GhostInfo.Three.WithAggro(AggressionType.PirateInvasion),
                [NPCID.PirateGhost] = GhostInfo.Three.WithAggro(AggressionType.PirateInvasion),
                [NPCID.MothronSpawn] = GhostInfo.Three.WithAggro(AggressionType.Eclipse),
                [NPCID.MothronEgg] = GhostInfo.Three.WithAggro(AggressionType.Eclipse),
                [NPCID.Butcher] = GhostInfo.Three.WithAggro(AggressionType.Eclipse),
                [NPCID.CreatureFromTheDeep] = GhostInfo.Three.WithAggro(AggressionType.Eclipse),
                [NPCID.Frankenstein] = GhostInfo.Three.WithAggro(AggressionType.Eclipse),
                [NPCID.Fritz] = GhostInfo.Three.WithAggro(AggressionType.Eclipse),
                [NPCID.Splinterling] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.Hellhound] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.Poltergeist] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.ZombieElf] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.ZombieElfBeard] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.ZombieElfGirl] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.GingerbreadMan] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.ElfArcher] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.Yeti] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.Nutcracker] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.NutcrackerSpinning] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.Flocko] = GhostInfo.Three.WithAggro(AggressionType.NightTime),
                [NPCID.ElfCopter] = GhostInfo.Three.WithAggro(AggressionType.NightTime),

                [NPCID.BoneLee] = GhostInfo.Four,
                [NPCID.BigMimicCorruption] = GhostInfo.Four,
                [NPCID.BigMimicCrimson] = GhostInfo.Four,
                [NPCID.BigMimicHallow] = GhostInfo.Four,
                [NPCID.DiabolistRed] = GhostInfo.Four,
                [NPCID.DiabolistWhite] = GhostInfo.Four,
                [NPCID.FloatyGross] = GhostInfo.Four,
                [NPCID.Wraith] = GhostInfo.Four,
                [NPCID.FlyingSnake] = GhostInfo.Four,
                [NPCID.GiantCursedSkull] = GhostInfo.Four,
                [NPCID.GiantFlyingFox] = GhostInfo.Four,
                [NPCID.GiantTortoise] = GhostInfo.Four,
                [NPCID.JungleCreeper] = GhostInfo.Four,
                [NPCID.JungleCreeperWall] = GhostInfo.Four,
                [NPCID.Lihzahrd] = GhostInfo.Four,
                [NPCID.LihzahrdCrawler] = GhostInfo.Four,
                [NPCID.MossHornet] = GhostInfo.Four,
                [NPCID.Moth] = GhostInfo.Four,
                [NPCID.Necromancer] = GhostInfo.Four,
                [NPCID.NecromancerArmored] = GhostInfo.Four,
                [NPCID.Paladin] = GhostInfo.Four,
                [NPCID.RaggedCaster] = GhostInfo.Four,
                [NPCID.RaggedCasterOpenCoat] = GhostInfo.Four,
                [NPCID.RedDevil] = GhostInfo.Four,
                [NPCID.RockGolem] = GhostInfo.Four,
                [NPCID.RuneWizard] = GhostInfo.Four,
                [NPCID.SkeletonCommando] = GhostInfo.Four,
                [NPCID.SkeletonSniper] = GhostInfo.Four,
                [NPCID.TacticalSkeleton] = GhostInfo.Four,
                [NPCID.GoblinShark] = GhostInfo.Four.WithAggro(AggressionType.NightTime),
                [NPCID.BloodNautilus] = GhostInfo.Four.WithAggro(AggressionType.BloodMoon),
                [NPCID.IceGolem] = GhostInfo.Four,
                [NPCID.SandElemental] = GhostInfo.Four,
                [NPCID.Mothron] = GhostInfo.Four.WithAggro(AggressionType.Eclipse),
                [NPCID.DeadlySphere] = GhostInfo.Four.WithAggro(AggressionType.Eclipse),
                [NPCID.DrManFly] = GhostInfo.Four.WithAggro(AggressionType.Eclipse),
                [NPCID.Eyezor] = GhostInfo.Four.WithAggro(AggressionType.Eclipse),
                [NPCID.Nailhead] = GhostInfo.Four.WithAggro(AggressionType.Eclipse),
                [NPCID.Psycho] = GhostInfo.Four.WithAggro(AggressionType.Eclipse),
                [NPCID.Reaper] = GhostInfo.Four.WithAggro(AggressionType.Eclipse),
                [NPCID.ThePossessed] = GhostInfo.Four.WithAggro(AggressionType.Eclipse),
                [NPCID.Vampire] = GhostInfo.Four.WithAggro(AggressionType.Eclipse),
                [NPCID.VampireBat] = GhostInfo.Four.WithAggro(AggressionType.Eclipse),
                [NPCID.BrainScrambler] = GhostInfo.Four.WithAggro(AggressionType.MartianMadness),
                [NPCID.GigaZapper] = GhostInfo.Four.WithAggro(AggressionType.MartianMadness),
                [NPCID.GrayGrunt] = GhostInfo.Four.WithAggro(AggressionType.MartianMadness),
                [NPCID.MartianEngineer] = GhostInfo.Four.WithAggro(AggressionType.MartianMadness),
                [NPCID.MartianOfficer] = GhostInfo.Four.WithAggro(AggressionType.MartianMadness),
                [NPCID.MartianWalker] = GhostInfo.Four.WithAggro(AggressionType.MartianMadness),
                [NPCID.RayGunner] = GhostInfo.Four.WithAggro(AggressionType.MartianMadness),
                [NPCID.Scutlix] = GhostInfo.Four.WithAggro(AggressionType.MartianMadness),
                [NPCID.MartianTurret] = GhostInfo.Four.WithAggro(AggressionType.MartianMadness),
                [NPCID.MartianSaucerCore] = GhostInfo.Four.WithAggro(AggressionType.MartianMadness),
                [NPCID.MourningWood] = GhostInfo.Four.WithAggro(AggressionType.NightTime),
                [NPCID.HeadlessHorseman] = GhostInfo.Four.WithAggro(AggressionType.NightTime),
                [NPCID.Krampus] = GhostInfo.Four.WithAggro(AggressionType.NightTime),
                [NPCID.PresentMimic] = GhostInfo.Four.WithAggro(AggressionType.NightTime),
                [NPCID.Everscream] = GhostInfo.Four.WithAggro(AggressionType.NightTime),
                [NPCID.SantaNK1] = GhostInfo.Four.WithAggro(AggressionType.NightTime),
                [NPCID.IceQueen] = GhostInfo.Four.WithAggro(AggressionType.NightTime),
                [NPCID.NebulaBeast] = GhostInfo.Four,
                [NPCID.NebulaBrain] = GhostInfo.Four,
                [NPCID.NebulaSoldier] = GhostInfo.Four,
                [NPCID.SolarCorite] = GhostInfo.Four,
                [NPCID.SolarDrakomire] = GhostInfo.Four,
                [NPCID.SolarSpearman] = GhostInfo.Four,
                [NPCID.SolarSolenian] = GhostInfo.Four,
                [NPCID.SolarSroller] = GhostInfo.Four,
                [NPCID.StardustJellyfishBig] = GhostInfo.Four,
                [NPCID.StardustSoldier] = GhostInfo.Four,
                [NPCID.StardustSpiderBig] = GhostInfo.Four,
                [NPCID.StardustSpiderSmall] = GhostInfo.Four,
                [NPCID.StardustWormHead] = GhostInfo.Four,
                [NPCID.VortexRifleman] = GhostInfo.Four,
                [NPCID.VortexSoldier] = GhostInfo.Four,
                [NPCID.AncientCultistSquidhead] = GhostInfo.Four,
                [NPCID.BigMimicJungle] = GhostInfo.Four,
                [NPCID.GoldenSlime] = GhostInfo.Four,

                [NPCID.DuneSplicerBody] = GhostInfo.Invalid,
                [NPCID.DuneSplicerTail] = GhostInfo.Invalid,
                [NPCID.Gnome] = GhostInfo.Invalid,
                [NPCID.MotherSlime] = GhostInfo.Invalid,
                [NPCID.GiantWormBody] = GhostInfo.Invalid,
                [NPCID.GiantWormTail] = GhostInfo.Invalid,
                [NPCID.BoneSerpentBody] = GhostInfo.Invalid,
                [NPCID.BoneSerpentTail] = GhostInfo.Invalid,
                [NPCID.DevourerBody] = GhostInfo.Invalid,
                [NPCID.DevourerTail] = GhostInfo.Invalid,
                [NPCID.TombCrawlerBody] = GhostInfo.Invalid,
                [NPCID.TombCrawlerTail] = GhostInfo.Invalid,
                [NPCID.DungeonGuardian] = GhostInfo.Invalid,
                [NPCID.DarkCaster] = GhostInfo.Invalid,
                [NPCID.FireImp] = GhostInfo.Invalid,
                [NPCID.ManEater] = GhostInfo.Invalid,
                [NPCID.Snatcher] = GhostInfo.Invalid,
                [NPCID.Tim] = GhostInfo.Invalid,
                [NPCID.AngryTrapper] = GhostInfo.Invalid,
            };

            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.Zombie, vanillaOnly: true))
            {
                NPCs[i] = GhostInfo.One.WithAggro(AggressionType.NightTime);
            }
            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.DemonEye, vanillaOnly: true))
            {
                NPCs[i] = GhostInfo.One.WithAggro(AggressionType.NightTime);
            }
            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.Hornet, vanillaOnly: true))
            {
                NPCs[i] = GhostInfo.Two;
            }
            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.AngryBones, vanillaOnly: true))
            {
                NPCs[i] = GhostInfo.Two;
            }
            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.Scarecrow1, vanillaOnly: true))
            {
                NPCs[i] = GhostInfo.Three.WithAggro(AggressionType.NightTime);
            }
            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.BlueArmoredBones, vanillaOnly: true))
            {
                NPCs[i] = GhostInfo.Four;
            }
            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.HellArmoredBones, vanillaOnly: true))
            {
                NPCs[i] = GhostInfo.Four;
            }
            foreach (var i in AequusHelpers.AllWhichShareBanner(NPCID.RustyArmoredBonesAxe, vanillaOnly: true))
            {
                NPCs[i] = GhostInfo.Four;
            }
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            InnerFinalizeContent();
        }
        public static void InnerFinalizeContent()
        {
            AutogenEntries();
            AutogenPriorities();
        }
        public static void AutogenEntries()
        {
            for (int i = Main.maxNPCTypes; i < NPCLoader.NPCCount; i++)
            {
                var n = ContentSamples.NpcsByNetId[i];
                var md = n.ModNPC;
                if (AutogeneratorIgnoreMods.Contains(md.Mod.Name) || NPCs.ContainsKey(n.type))
                {
                    continue;
                }

                NPCs.Add(i, GhostInfo.Autogenerate(n));
            }
        }
        public static void AutogenPriorities()
        {
            foreach (int i in NPCs.Keys)
            {
                var g = NPCs[i];
                var n = ContentSamples.NpcsByNetId[i];
                g.despawnPriority = CalcPriority(CalcDamageTiering(g.PowerNeeded, n.boss, n.noGravity), n.life, n.damage, n.defense, g.slotsUsed.GetValueOrDefault(1));
                NPCs[i] = g;
            }
        }
        public static float CalcDamageTiering(float powerNeeded, bool boss, bool flies)
        {
            float tiering = powerNeeded;
            if (boss)
            {
                tiering += 10f;
            }
            if (flies)
            {
                tiering *= 2f;
            }
            return tiering;
        }
        public static int CalcPriority(float tiering, int life, int damage, int defense, int slotsUsed)
        {
            return (int)((life + damage * 3 + defense * 2) * slotsUsed * tiering);
        }

        /// <summary>
        /// Adds a NecroStats data for an npc index in <see cref="NPCs"/>
        /// <para>Parameter 1: NPC Type (short)</para>
        /// <para>Parameter 2: Tier (float), <see cref="ZombieScepter"/> is tier 1, <see cref="Insurgency"/> is tier 4</para>
        /// <para>Parameter 3 (Optional): View range (float), how close a slave needs to be to an enemy in order for it to target it. Defaults to 800</para>
        /// <para>Parameter 4+ (Optional): Two paired arguments. One string and one value</para>
        /// <para>A successful mod call would look like:</para>
        /// <code>aequus.Call("NecroStats", ModContent.NPCType{...}(), 1f);</code> OR
        /// <code>aequus.Call("NecroStats", ModContent.NPCType{...}(), 1f, 800f);</code> OR
        /// <code>aequus.Call("NecroStats", ModContent.NPCType{...}(), 1f, 800f, "PrioritizePlayerMultiplier", 4f);</code>
        /// <para>Please handle these mod calls in <see cref="Mod.PostSetupContent"/>. As buff immunities are setup in <see cref="InnerFinalizeContent"/></para>
        /// </summary>
        /// <param name="aequus"></param>
        /// <param name="args"></param>
        /// <returns>'Success' when correctly handled. 'Failure' when improperly handled</returns>
        public object HandleModCall(Aequus aequus, object[] args)
        {
            if (args[1] is string option)
            {
                if (option == "AddToBlacklist")
                {
                    if (args[2] is Mod ignoreMod)
                    {
                        AutogeneratorIgnoreMods.Add(ignoreMod.Name);
                        return IModCallable.Success;
                    }
                    else if (args[2] is string ignoreModName)
                    {
                        AutogeneratorIgnoreMods.Add(ignoreModName);
                        return IModCallable.Success;
                    }
                }
                return IModCallable.Failure;
            }

            int npc = 0;
            try
            {
                npc = IModCallable.UnboxIntoInt(args[1]);
                float tier = IModCallable.UnboxIntoFloat(args[2]);
                float viewDistance = args.Length >= 4 ? IModCallable.UnboxIntoFloat(args[3]) : 800f;
                if (Aequus.LogMore)
                    Aequus.Instance.Logger.Info("Adding necromancy data for: " + Lang.GetNPCName(npc) + " -- Tier: " + tier + ", SightRange: " + viewDistance + " --");
                var stats = new GhostInfo(tier, viewDistance);
                stats = (GhostInfo)IModCallHandler.HandleArgs(stats, args.Length >= 4 ? 4 : 3, args);
                NPCs.Set(npc, stats);
            }
            catch (Exception ex)
            {
                string name = "Unknown";
                if (npc > NPCID.NegativeIDCount && npc < NPCLoader.NPCCount)
                {
                    name = Lang.GetNPCName(npc).Value;
                }
                Aequus.Instance.Logger.Error("Failed handling a mod call for NecroStats. {NPC Type: " + npc + ", Potential Name: " + name + "}", ex);
                return IModCallable.Failure;
            }

            return IModCallable.Success;
        }

        /// <summary>
        /// Attempts to get the data for this NPC ID, otherwise throws an exception. Does not take netIDs into account
        /// <para>If you are wanting to use a safer method, please use <see cref="TryGet(int, out GhostInfo)"/></para>
        /// </summary>
        /// <param name="type">the NPC ID.</param>
        /// <returns></returns>
        public static GhostInfo Get(int type)
        {
            return NPCs[type];
        }
        /// <summary>
        /// Attempts to get the data for this NPC ID, otherwise throws an exception. Does not take netIDs into account
        /// <para>If you are wanting to use a safer method, please use <see cref="TryGet(NPC, out GhostInfo)"/></para>
        /// </summary>
        /// <param name="npc">the NPC instance</param>
        /// <returns></returns>
        public static GhostInfo Get(NPC npc)
        {
            return Get(npc.type);
        }

        /// <summary>
        /// Tries to get a <see cref="GhostInfo"/> value using the given NPC Net ID. Returns false if no info exists, and the popped <see cref="GhostInfo"/> would be useless.
        /// </summary>
        /// <param name="netID">The Net ID (Basically the same as npc.type, but is negative for specialized enemies)</param>
        /// <param name="value">The released <see cref="GhostInfo"/> value, when this method returns false this is set to default(GhostInfo)</param>
        /// <returns>Whether or not there is <see cref="GhostInfo"/> related to this NPC Net ID</returns>
        public static bool TryGet(int netID, out GhostInfo value)
        {
            if (netID < 0 && NPCs.ContainsKey(netID))
            {
                value = Get(netID);
                return true;
            }
            return NPCs.TryGetValue(NPCID.FromNetId(netID), out value);
        }
        /// <summary>
        /// Tries to get a <see cref="GhostInfo"/> value using the given NPC. Returns false if no info exists, and the popped <see cref="GhostInfo"/> would be useless.
        /// </summary>
        /// <param name="npc">The NPC instance</param>
        /// <param name="value">The released <see cref="GhostInfo"/> value, when this method returns false this is set to default(GhostInfo)</param>
        /// <returns>Whether or not there is <see cref="GhostInfo"/> related to this NPC Net ID</returns>
        public static bool TryGet(NPC npc, out GhostInfo value)
        {
            return TryGet(npc.netID, out value);
        }
    }
}