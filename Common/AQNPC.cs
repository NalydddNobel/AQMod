using AQMod.Common.Utilities;
using AQMod.Content;
using AQMod.Content.Dusts;
using AQMod.Content.RobsterQuests;
using AQMod.Content.WorldEvents.AzureCurrents;
using AQMod.Content.WorldEvents.CrabSeason;
using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.Amulets;
using AQMod.Items.Accessories.FishingSeals;
using AQMod.Items.Dedicated;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Tools;
using AQMod.Items.Tools.MapMarkers;
using AQMod.Items.Vanities.CursorDyes;
using AQMod.Localization;
using AQMod.NPCs.Friendly.Town;
using AQMod.NPCs.Monsters;
using AQMod.NPCs.Monsters.AtmosphericEvent;
using AQMod.NPCs.Monsters.DemonicEvent;
using AQMod.Projectiles.Monster;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class AQNPC : GlobalNPC
    {
        public static class Sets
        {
            public static bool[] NoSpoilLoot { get; private set; }
            public static bool[] NoMapBlip { get; private set; }
            public static bool[] NoGlobalDrops { get; private set; }
            public static bool[] HecktoplasmDungeonEnemy { get; private set; }
            public static bool[] EnemyDungeonSprit { get; private set; }
            public static bool[] DemonSiegeEnemy { get; private set; }

            public static bool IsAZombie(int type, bool includeBloodZombies = false)
            {
                if (includeBloodZombies && type == NPCID.BloodZombie)
                {
                    return true;
                }
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

            public static int CountNPCs(bool[] ruleset)
            {
                int count = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && ruleset[Main.npc[i].type])
                    {
                        count++;
                    }
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

            internal static void Setup()
            {
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
            }

            internal static void Unload()
            {
                DemonSiegeEnemy = null;
                EnemyDungeonSprit = null;
                HecktoplasmDungeonEnemy = null;
                NoSpoilLoot = null;
                NoMapBlip = null;
                NoGlobalDrops = null;
            }
        }

        public static bool CanDropEnergy => NPC.downedBoss1 && !NoEnergyDrops;
        internal static Color GreenSlimeColor => new Color(0, 220, 40, 100);
        internal static Color BlueSlimeColor => new Color(0, 80, 255, 100);

        public static bool NoEnergyDrops { get; set; }

        public static bool BossRush { get; private set; }
        public static byte BossRushPlayer { get; private set; }

        private static bool _showEnergyDropsMessage;
        public override bool InstancePerEntity => true;

        public bool sparkling;
        public bool notFrostburn;

        public static int MeteorLength = 7200; // 2 minutes, making each meteor time 4 minutes

        public static void CrashMeteor(int x, int y, int size)
        {
        }

        public static bool CheckStariteDeath(NPC npc)
        {
            return Main.dayTime;
        }

        public override void ResetEffects(NPC npc)
        {
            sparkling = false;
            notFrostburn = false;
        }

        public static bool AreTheSameNPC(int type, int otherType)
        {
            switch (type)
            {
                default:
                return type == otherType;

                case NPCID.BlueArmoredBones:
                case NPCID.BlueArmoredBonesMace:
                case NPCID.BlueArmoredBonesNoPants:
                case NPCID.BlueArmoredBonesSword:
                {
                    switch (otherType)
                    {
                        default:
                        return false;

                        case NPCID.BlueArmoredBones:
                        case NPCID.BlueArmoredBonesMace:
                        case NPCID.BlueArmoredBonesNoPants:
                        case NPCID.BlueArmoredBonesSword:
                        return true;
                    }
                }

                case NPCID.AngryBones:
                case NPCID.AngryBonesBig:
                case NPCID.AngryBonesBigHelmet:
                case NPCID.AngryBonesBigMuscle:
                {
                    switch (otherType)
                    {
                        default:
                        return false;

                        case NPCID.AngryBones:
                        case NPCID.AngryBonesBig:
                        case NPCID.AngryBonesBigHelmet:
                        case NPCID.AngryBonesBigMuscle:
                        return true;
                    }
                }

                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                {
                    switch (otherType)
                    {
                        default:
                        return false;

                        case NPCID.Necromancer:
                        case NPCID.NecromancerArmored:
                        return true;
                    }
                }

                case NPCID.DiabolistRed:
                case NPCID.DiabolistWhite:
                {
                    switch (otherType)
                    {
                        default:
                        return false;

                        case NPCID.DiabolistRed:
                        case NPCID.DiabolistWhite:
                        return true;
                    }
                }

                case NPCID.HellArmoredBones:
                case NPCID.HellArmoredBonesMace:
                case NPCID.HellArmoredBonesSpikeShield:
                case NPCID.HellArmoredBonesSword:
                {
                    switch (otherType)
                    {
                        default:
                        return false;

                        case NPCID.HellArmoredBones:
                        case NPCID.HellArmoredBonesMace:
                        case NPCID.HellArmoredBonesSpikeShield:
                        case NPCID.HellArmoredBonesSword:
                        return true;
                    }
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (sparkling)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 10;
                if (damage < 1)
                    damage = 1;
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (npc.townNPC)
            {
                if (projectile.type == ModContent.ProjectileType<OmegaRay>())
                {
                    damage = (int)(damage * 0.1f);
                }
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (MoonlightWallHelper.Instance.Active) // in case the NPC before this one broke and skipped PostAI, if there's a next NPC then it would hopefully fix it
            {
                MoonlightWallHelper.Instance.End();
            }
            if (MoonlightWallHelper.BehindMoonlightWall(npc.Center))
            {
                MoonlightWallHelper.Instance.Begin();
            }
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

                case NPCID.DungeonSpirit:
                {
                    if (Main.netMode == NetmodeID.SinglePlayer && Main.LocalPlayer.GetModPlayer<AQPlayer>().spiritAmulet)
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
            if (MoonlightWallHelper.Instance.Active)
            {
                MoonlightWallHelper.Instance.End();
            }
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

        // Main.moonPhase:
        // 0 = Full Moon
        // 1 = Waning Gibbious
        // 2 = Third Quarter
        // 3 = Waning Crescent
        // 4 = New Moon
        // 5 = Waxing Crescent
        // 6 = First Quarter
        // 7 = Waxing Gibbious
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            switch (type)
            {
                case NPCID.Merchant:
                {
                    for (int i = 0; i < Main.maxInventory; i++)
                    {
                        if (Main.player[Main.myPlayer].inventory[i].useAmmo == AmmoID.Dart)
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.Seed);
                            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(copper: 10);
                            nextSlot++;
                            break;
                        }
                    }
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.BuffItems.SpoilsPotion>());
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Tools.Powders.GoldPowder>());
                        nextSlot++;
                    }
                }
                break;

                case NPCID.Dryad:
                {
                    var plr = Main.LocalPlayer;
                    if (Main.hardMode && NPC.downedPlantBoss)
                    {
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<Baguette>());
                        nextSlot++;
                    }
                }
                break;

                case NPCID.Clothier:
                if (Main.eclipse)
                {
                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Vanities.MonoxideHat>());
                    nextSlot++;
                }
                break;

                case NPCID.PartyGirl:
                {
                    if (!Main.dayTime && Main.moonPhase == 0)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.WhoopieCushion);
                        nextSlot++;
                    }
                }
                break;

                case NPCID.Pirate:
                {
                    if (Main.player[Main.myPlayer].anglerQuestsFinished >= 20)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.AnglerPants);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.AnglerVest);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.AnglerHat);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<CopperSeal>());
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<SilverSeal>());
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<GoldSeal>());
                        nextSlot++;
                    }
                    else if (Main.player[Main.myPlayer].anglerQuestsFinished >= 15)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.AnglerVest);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.AnglerHat);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<CopperSeal>());
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<SilverSeal>());
                        nextSlot++;
                    }
                    else if (Main.player[Main.myPlayer].anglerQuestsFinished >= 10)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.AnglerHat);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<CopperSeal>());
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<SilverSeal>());
                        nextSlot++;
                    }
                    else if (Main.player[Main.myPlayer].anglerQuestsFinished >= 2)
                    {
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<CopperSeal>());
                        nextSlot++;
                    }
                }
                break;

                case NPCID.SkeletonMerchant:
                {
                    var plr = Main.LocalPlayer;
                    if (!Main.dayTime)
                    {
                        switch (Main.moonPhase)
                        {
                            case 0:
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.MiningPotion);
                                nextSlot++;
                            }
                            break;

                            case 1:
                            case 2:
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.MiningShirt);
                                nextSlot++;
                            }
                            break;

                            case 3:
                            case 4:
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.MiningPants);
                                nextSlot++;
                            }
                            break;

                            case 5:
                            case 6:
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<MinersFlashlight>());
                                nextSlot++;
                            }
                            break;
                        }
                    }
                }
                break;
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
            {
                return;
            }
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                var player = Main.player[k];
                var aQPlayer = player.GetModPlayer<AQPlayer>();
                if (Main.netMode == NetmodeID.Server)
                {
                    aQPlayer.NetUpdateKillCount = true;
                    aQPlayer.SyncPlayer(-1, player.whoAmI, false);
                }
                if (!aQPlayer.bossrush)
                {
                    aQPlayer.CurrentEncoreKillCount[npc.type] = 0;
                    return;
                }
                aQPlayer.CurrentEncoreKillCount[npc.type]++;
                if (aQPlayer.CurrentEncoreKillCount[npc.type] > aQPlayer.EncoreBossKillCountRecord[npc.type])
                {
                    aQPlayer.EncoreBossKillCountRecord[npc.type] = aQPlayer.CurrentEncoreKillCount[npc.type];
                }
                CombatText.NewText(player.getRect(), Main.mouseColor, aQPlayer.CurrentEncoreKillCount[npc.type], true);
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
                float yOff = boss.height * 2f;
                float healthScale = bossRushScale(npc.lifeMax);
                float damageScale = MathHelper.Clamp(healthScale, 1.01f, 1.15f);
                boss.lifeMax = (int)(npc.lifeMax * healthScale);
                boss.life = boss.lifeMax;
                boss.defDamage = (int)(npc.defDamage * damageScale);
                boss.damage = boss.defDamage;
                Vector2 spawnPosition = new Vector2(x, player.position.Y - yOff);
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
            }
        }

        private void ManageSpectreCharm(NPC npc)
        {
            int soulCollector = -1;
            float soulCollectorDistance = 2000f;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var plr = Main.player[i];
                if (plr.active && !plr.dead)
                {
                    var aQPlr = plr.GetModPlayer<AQPlayer>();
                    float distance = Vector2.Distance(plr.Center, npc.Center);
                    if (aQPlr.spectreSoulCollector && distance < soulCollectorDistance)
                    {
                        soulCollector = i;
                        soulCollectorDistance = distance;
                    }
                }
            }
            if (soulCollector == -1)
                return;
            if (npc.boss)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (Main.rand.NextBool(2))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<Items.SpectreSoul>());
                }
            }
            else
            {
                var soulCollecterCenter = Main.player[soulCollector].Center;
                int lowestPercentPlayer = soulCollector;
                float lifePercent = Main.player[soulCollector].statLife / (float)Main.player[soulCollector].statLifeMax2;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (i == soulCollector || Main.player[i].dead || !Main.player[i].active)
                        continue;
                    if (Vector2.Distance(soulCollecterCenter, Main.player[i].Center) < 2000f)
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
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Items.SpectreSoul>());
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
                    if (aQPlr.dreadsoul && distance < 2000f && plr.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.DreadsoulAttack>()] < 20)
                    {
                        var center = npc.Center;
                        float rot = MathHelper.TwoPi / 3f;
                        int damage = (int)(plr.GetWeaponDamage(plr.HeldItem) * 0.65f);
                        float kb = plr.GetWeaponKnockback(plr.HeldItem, 1f) / 3f;
                        float size = (float)Math.Sqrt(npc.width * npc.height);
                        for (int j = 0; j < 3; j++)
                        {
                            var dir = new Vector2(0f, -1f).RotatedBy(rot * j);
                            Projectile.NewProjectile(center + dir * (size + 2f), dir * 2f, ModContent.ProjectileType<Projectiles.DreadsoulAttack>(), damage, kb, i, 0f, npc.type);
                        }
                        plr.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.DreadsoulAttack>()] += 3;
                        break;
                    }
                }
            }
        }

        public override bool PreNPCLoot(NPC npc)
        {
            _showEnergyDropsMessage = !NPC.downedBoss1;
            if (_loop != 0)
            {
                NPCLoader.blockLoot.Add(ItemID.Heart);
            }
            else
            {
                ManageSpectreCharm(npc);
                if (npc.whoAmI == HuntSystem.TargetNPC)
                {
                    if (HuntSystem.Hunt != null)
                    {
                        HuntSystem.Hunt.RemoveHunt();
                    }
                    HuntSystem.RandomizeHunt(null);
                    AQMod.BroadcastMessage("Mods.AQMod.Common.RobsterNPCDeath", Robster.RobsterBroadcastMessageColor);
                    AQMod.BroadcastMessage("Mods.AQMod.Common.RobsterNPCDeath2", Robster.RobsterBroadcastMessageColor);
                }
            }
            return true;
        }

        public override bool SpecialNPCLoot(NPC npc)
        {
            if (Sets.HecktoplasmDungeonEnemy[npc.type] && npc.lifeMax > 100)
            {
                if (npc.HasPlayerTarget && Main.hardMode && NPC.downedPlantBoss && Main.player[npc.target].ZoneDungeon)
                {
                    int spawnChance = 13;
                    if (Main.expertMode)
                        spawnChance = 9;
                    var center = npc.Center;
                    if (Main.wallDungeon[Main.tile[(int)center.X / 16, (int)center.Y / 16].wall] && Main.rand.Next(spawnChance) == 0)
                    {
                        int n = NPC.NewNPC((int)center.X, (int)center.Y, ModContent.NPCType<Heckto>());
                    }
                }
                npc.lifeMax = 99;
                npc.NPCLoot();
                return true;
            }
            return false;
        }

        private static byte _loop;

        public override void NPCLoot(NPC npc)
        {
            byte p = Player.FindClosest(npc.position, npc.width, npc.height);
            var plr = Main.player[p];
            AQPlayer aQPlayer = plr.GetModPlayer<AQPlayer>();
            if (npc.SpawnedFromStatue || NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type])
                return;
            if (!npc.boss && !npc.friendly)
            {
                if (!Sets.NoSpoilLoot[npc.type] && _loop < aQPlayer.spoiled)
                {
                    _loop++;
                    npc.NPCLoot();
                    _loop = 0;
                }
            }
            if (_loop == 0)
            {
                ManageDreadsoul(npc);
                EncoreKill(npc);
            }
            if (npc.townNPC && npc.position.Y > (Main.maxTilesY - 200) * 16f)
            {
                Rectangle check = new Rectangle((int)npc.position.X / 16, (int)npc.position.Y / 16, 2, 3);
                for (int i = check.X; i <= check.X + check.Width; i++)
                {
                    for (int j = check.Y; j <= check.Y + check.Height; j++)
                    {
                        if (Framing.GetTileSafely(i, j).liquid > 0 && Main.tile[i, j].lava())
                        {
                            Item.NewItem(npc.getRect(), ModContent.ItemType<Baguette>());
                            break;
                        }
                    }
                }
            }
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (Sets.EnemyDungeonSprit[npc.type])
                {
                    if (!aQPlayer.spiritAmuletHeld)
                    {
                        int count = Sets.CountNPCs(Sets.EnemyDungeonSprit);
                        if (count > 1)
                        {
                            int itemType = ModContent.ItemType<SpiritAmulet>();
                            if (!AQItem.ItemOnGroundAlready(itemType))
                            {
                                int chance = 8 - count * 2;
                                if (chance <= 1 || Main.rand.NextBool(chance))
                                {
                                    Item.NewItem(npc.getRect(), itemType);
                                }
                            }
                        }
                    }
                }
                else if (npc.type == NPCID.Ghost)
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
                    if (aQPlayer.opposingForce && Main.rand.NextBool(5))
                    {
                        if (plr.ZoneCorrupt || plr.ZoneCrimson)
                            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofLight);
                        if (plr.ZoneHoly)
                            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofNight);
                    }
                }
                if (NPC.downedBoss1 && !NoEnergyDrops)
                {
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
            }
            if (npc.type >= Main.maxNPCTypes)
                return;
            if (npc.type == NPCID.AngryBones || npc.type == NPCID.DarkCaster || npc.type == NPCID.CursedSkull)
            {
                if (Main.rand.NextBool(75))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<DungeonMap>());
            }
            if (npc.type == NPCID.Lihzahrd || npc.type == NPCID.LihzahrdCrawler || npc.type == NPCID.FlyingSnake)
            {
                if (Main.rand.NextBool(50))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<LihzahrdMap>());
            }
            switch (npc.type)
            {
                case NPCID.UndeadViking:
                {
                    if (Main.rand.NextBool(6))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Weapons.Melee.CrystalDagger>());
                }
                break;

                case NPCID.EyeofCthulhu:
                {
                    if (_showEnergyDropsMessage)
                    {
                        NoEnergyDrops = false;
                        _showEnergyDropsMessage = false;
                        AQMod.BroadcastMessage(AQText.Key + "Common.EnergyDoDrop", new Color(80, 200, 255, 255));
                    }
                }
                break;

                case NPCID.Harpy:
                {
                    if (CanDropEnergy && Main.rand.NextBool(8))
                        Item.NewItem(npc.getRect(), ModContent.ItemType<AtmosphericEnergy>());
                }
                break;

                case NPCID.Crab:
                {
                    if (CrabSeason.Active)
                    {
                        if ((Main.moonPhase % 2 == 0 && !aQPlayer.opposingForce) || (Main.moonPhase % 2 == 1 && aQPlayer.opposingForce))
                        {
                            Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>());
                        }
                    }
                    else
                    {
                        bool hasItem = Main.player[p].HasItem(ModContent.ItemType<CrabClock>());
                        if (Main.rand.NextBool(hasItem ? 10 : 4))
                        {
                            Item.NewItem(npc.getRect(), ModContent.ItemType<CrabClock>());
                        }
                        if (Main.rand.NextBool())
                        {
                            Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>());
                        }
                    }
                }
                break;

                case NPCID.DarkMummy:
                {
                    if (aQPlayer.opposingForce && Main.rand.NextBool(10))
                        Item.NewItem(npc.getRect(), ItemID.LightShard);
                }
                break;

                case NPCID.LightMummy:
                {
                    if (aQPlayer.opposingForce && Main.rand.NextBool(10))
                        Item.NewItem(npc.getRect(), ItemID.DarkShard);
                }
                break;

                case NPCID.DungeonSpirit:
                if (Main.rand.NextBool(30))
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Breadsoul>());
                }
                break;

                case NPCID.GingerbreadMan:
                if (Main.rand.NextBool(1000))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Baguette>());
                break;

                case NPCID.Mothron:
                if (NPC.downedAncientCultist && Main.rand.NextBool(3))
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

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.GetModPlayer<AQPlayer>().bossrush)
            {
                spawnRate *= 10;
                maxSpawns = (int)(maxSpawns * 0.1);
            }
            else if (AQMod.ShouldReduceSpawns())
            {
                spawnRate = 1000;
                maxSpawns = 0;
                return;
            }
            if (DemonSiege.CloseEnoughToDemonSiege(player))
            {
                spawnRate *= 10;
                maxSpawns = (int)(maxSpawns * 0.1);
            }
            else
            {
                if (player.position.Y < AQMod.SpaceLayer - (40 * 16f))
                {
                    if (AzureCurrents.MeteorTime())
                    {
                        spawnRate /= 2;
                        maxSpawns *= 2;
                    }
                }
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (AQMod.CosmicEvent.SpawnsActive(spawnInfo.player))
            {
                int tileDistance = AQMod.CosmicEvent.GetTileDistance(spawnInfo.player);
                if (tileDistance < GlimmerEvent.MaxDistance)
                {
                    if (tileDistance > GlimmerEvent.HyperStariteDistance) // shouldn't divide by 0...
                    {
                        float normalSpawnsMult = 1f - (1f / (tileDistance - GlimmerEvent.HyperStariteDistance));
                        IEnumerator<int> keys = pool.Keys.GetEnumerator();
                        int[] keyValue = new int[pool.Count];
                        for (int i = 0; i < pool.Count; i++)
                        {
                            keyValue[i] = keys.Current;
                            if (!keys.MoveNext())
                            {
                                break;
                            }
                        }
                        keys.Dispose();
                        for (int i = 0; i < pool.Count; i++)
                        {
                            pool[keyValue[i]] *= normalSpawnsMult;
                        }
                    }
                    else
                    {
                        int[] keyValue = new int[pool.Count];
                        IEnumerator<int> keys = pool.Keys.GetEnumerator();
                        for (int i = 0; i < pool.Count; i++)
                        {
                            keyValue[i] = keys.Current;
                            if (!keys.MoveNext())
                            {
                                break;
                            }
                        }
                        for (int i = 0; i < pool.Count; i++)
                        {
                            pool[keyValue[i]] = 0f;
                        }
                    }
                    int layerIndex = GlimmerEvent.GetLayerIndex(tileDistance);
                    if (layerIndex != -1)
                    {
                        for (int i = layerIndex - 1; i >= 0; i--)
                        {
                            pool.Add(GlimmerEvent.Layers[i].NPCType, GlimmerEvent.Layers[i].SpawnChance);
                        }
                        if (layerIndex == GlimmerEvent.Layers.Count - 1)
                        {
                            pool.Add(GlimmerEvent.Layers[layerIndex].NPCType, (AQUtils.GetGrad(0, GlimmerEvent.Layers[layerIndex].Distance, tileDistance) * GlimmerEvent.Layers[layerIndex].SpawnChance));
                        }
                        else
                        {
                            pool.Add(GlimmerEvent.Layers[layerIndex].NPCType, 1f - (AQUtils.GetGrad(GlimmerEvent.Layers[layerIndex + 1].Distance, GlimmerEvent.Layers[layerIndex].Distance, tileDistance) * GlimmerEvent.Layers[layerIndex].SpawnChance));
                        }
                    }
                }
            }
            else
            {
                if (spawnInfo.spawnTileY < AQMod.SpaceLayerTile - 40)
                {
                    if (AzureCurrents.MeteorTime())
                    {
                        //pool.Clear();
                        //pool.Add(ModContent.NPCType<Meteor>(), 1f);
                    }
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (sparkling)
            {
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, ModContent.DustType<UltimaDust>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), Main.rand.NextFloat(0.6f, 1.25f));
                    Main.dust[dust].velocity *= 2.65f;
                    Main.dust[dust].velocity.Y -= 2f;
                }
                float positionLength = npc.Center.Length() / 32f;
                const float offset = MathHelper.TwoPi / 3f;
                var r = 1 * ((float)Math.Sin(positionLength) + 1f);
                var g = 1 * ((float)Math.Sin(positionLength + offset) + 1f);
                var b = 1 * ((float)Math.Sin(positionLength + offset * 2f) + 1f);
                Lighting.AddLight(npc.Center, r * 0.25f, g * 0.25f, b * 0.25f);
            }
        }

        internal static int FindClosest(Vector2 position, float distance = 2000f)
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
                {
                    continue;
                }
                if (isColliding(Main.npc[i]))
                {
                    onCollide(Main.npc[i]);
                }
            }
        }
    }
}