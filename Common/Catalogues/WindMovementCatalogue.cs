﻿using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Common.Catalogues
{
    public sealed class WindMovementCatalogue : CatalogueBase
    {
        public static HashSet<int> NPCsWhitelist { get; private set; }
        public static HashSet<int> ProjectilesWhitelist { get; private set; }

        public override void InitalizeMiscEntries()
        {
            NPCsWhitelist = new HashSet<int>();
            ProjectilesWhitelist = new HashSet<int>();
        }

        public override void LoadAutomaticEntries()
        {
            AutomaticEntries_NPCs(new HashSet<int>()
            {
                0,
                NPCAIStyleID.Slime,
                NPCAIStyleID.DemonEye,
                NPCAIStyleID.Fighter,
                NPCAIStyleID.Fairy,
                NPCAIStyleID.AncientLight,
                NPCAIStyleID.BabyMothron,
                NPCAIStyleID.Balloon,
                NPCAIStyleID.Bat,
                NPCAIStyleID.Bird,
                NPCAIStyleID.Butterfly,
                NPCAIStyleID.Caster,
                NPCAIStyleID.Creeper,
                NPCAIStyleID.CritterWorm,
                NPCAIStyleID.Dragonfly,
                NPCAIStyleID.Duck,
                NPCAIStyleID.DukeFishronBubble,
                NPCAIStyleID.ElfCopter,
                NPCAIStyleID.EnchantedSword,
                NPCAIStyleID.Firefly,
                NPCAIStyleID.Flying,
                NPCAIStyleID.FlyingFish,
                NPCAIStyleID.GiantTortoise,
                NPCAIStyleID.GraniteElemental,
                NPCAIStyleID.Herpling,
                NPCAIStyleID.HoveringFighter,
                NPCAIStyleID.Jellyfish,
                NPCAIStyleID.Ladybug,
                NPCAIStyleID.ManEater,
                NPCAIStyleID.Mimic,
                NPCAIStyleID.MothronEgg,
                NPCAIStyleID.NebulaFloater,
                NPCAIStyleID.Passive,
                NPCAIStyleID.Piranha,
                NPCAIStyleID.PlanteraTentacle,
                NPCAIStyleID.Seahorse,
                NPCAIStyleID.SmallStarCell,
                NPCAIStyleID.Spell,
                NPCAIStyleID.Spider,
                NPCAIStyleID.Spore,
                NPCAIStyleID.StarCell,
                NPCAIStyleID.TheHungry,
                NPCAIStyleID.Unicorn,
                NPCAIStyleID.Vulture,
                NPCAIStyleID.WaterStrider,
            });
            AutomaticEntries_Projectiles(new HashSet<int>()
            {
                0,
                ProjAIStyleID.Arrow,
                ProjAIStyleID.ThrownProjectile,
                ProjAIStyleID.Hook,
                ProjAIStyleID.Sickle,
                ProjAIStyleID.Boulder,
                ProjAIStyleID.Boomerang,
                ProjAIStyleID.Explosive,
                ProjAIStyleID.Bounce,
                ProjAIStyleID.CrystalShard,
                ProjAIStyleID.Bubble,
                ProjAIStyleID.Bobber,
                ProjAIStyleID.BeachBall,
                ProjAIStyleID.ArcanumSubShot,
                ProjAIStyleID.CelebrationMk2Shots,
                ProjAIStyleID.Chum,
                ProjAIStyleID.ColdBolt,
                ProjAIStyleID.CommonFollow,
                ProjAIStyleID.CrystalLeafShot,
                ProjAIStyleID.DD2FlameBurstShot,
                ProjAIStyleID.DesertTiger,
                ProjAIStyleID.DesertTigerBall,
                ProjAIStyleID.DrakomiteFlare,
                ProjAIStyleID.ExplosiveBunny,
                ProjAIStyleID.FairyGlowStick,
                ProjAIStyleID.FallingStar,
                ProjAIStyleID.FallingTile,
                ProjAIStyleID.FireWork,
                ProjAIStyleID.FireWorkFountain,
                ProjAIStyleID.Flail,
                ProjAIStyleID.Flairon,
                ProjAIStyleID.Flairon,
                ProjAIStyleID.Flames,
                ProjAIStyleID.FlameThrower,
                ProjAIStyleID.FlamingJack,
                ProjAIStyleID.FlamingScythe,
                ProjAIStyleID.FloatingFollow,
                ProjAIStyleID.FlowerPetal,
                ProjAIStyleID.FlyingPiggyBank,
                ProjAIStyleID.GemStaffBolt,
                ProjAIStyleID.Ghast,
                ProjAIStyleID.GraveMarker,
                ProjAIStyleID.GroundProjectile,
                ProjAIStyleID.Harpoon,
                ProjAIStyleID.Hornet,
                ProjAIStyleID.IchorSplash,
                ProjAIStyleID.Inferno,
                ProjAIStyleID.InfluxWaver,
                ProjAIStyleID.Kite,
                ProjAIStyleID.Leaf,
                ProjAIStyleID.MagicLantern,
                ProjAIStyleID.MagnetSphere,
                ProjAIStyleID.MartianRocket,
                ProjAIStyleID.MechanicWrench,
                ProjAIStyleID.MiniTwins,
                ProjAIStyleID.MolotovCocktail,
                ProjAIStyleID.MoveShort,
                ProjAIStyleID.Mushroom,
                ProjAIStyleID.MusicNote,
                ProjAIStyleID.Nail,
                ProjAIStyleID.NebulaArcanum,
                ProjAIStyleID.NurseSyringe,
                ProjAIStyleID.PaperPlane,
                ProjAIStyleID.Pet,
                ProjAIStyleID.Powder,
                ProjAIStyleID.Raven,
                ProjAIStyleID.ReleasedProjectile,
                ProjAIStyleID.RopeCoil,
                ProjAIStyleID.ScutlixLaser,
                ProjAIStyleID.SmallFlying,
                ProjAIStyleID.Spray,
                ProjAIStyleID.Stream,
                ProjAIStyleID.SuperStar,
                ProjAIStyleID.TerrarianBeam,
                ProjAIStyleID.ToiletEffect,
                ProjAIStyleID.ToxicBubble,
                ProjAIStyleID.VoidBag,
                ProjAIStyleID.WaterJet,
                ProjAIStyleID.WireKite,
            });
        }
        private void AutomaticEntries_NPCs(HashSet<int> hash)
        {
            foreach (var n in ContentSamples.NpcsByNetId)
            {
                if (NPCsWhitelist.Contains(n.Key))
                {
                    continue;
                }
                try
                {
                    var npc = n.Value;
                    if (hash.Contains(npc.aiStyle))
                    {
                        NPCsWhitelist.Add(n.Key);
                    }
                }
                catch (Exception e)
                {
                    var l = Aequus.Instance.Logger;
                    l.Error("An error occured when doing algorithmic checks for sets for {" + Lang.GetNPCName(n.Key).Value + ", ID: " + n.Key + "}", e);
                }
            }
        }
        private void AutomaticEntries_Projectiles(HashSet<int> hash)
        {
            foreach (var p in ContentSamples.ProjectilesByType)
            {
                if (ProjectilesWhitelist.Contains(p.Key))
                {
                    continue;
                }
                try
                {
                    var projectile = p.Value;
                    if (hash.Contains(projectile.aiStyle))
                    {
                        ProjectilesWhitelist.Add(p.Key);
                    }
                }
                catch (Exception e)
                {
                    var l = Aequus.Instance.Logger;
                    l.Error("An error occured when doing algorithmic checks for sets for {" + Lang.GetProjectileName(p.Key).Value + ", ID: " + p.Key + "}", e);
                }
            }
        }

        public override void RemoveUnwantedEntries()
        {
            NPCsWhitelist.Remove(NPCID.BloodSquid);
        }

        public override void Unload()
        {
            NPCsWhitelist?.Clear();
            NPCsWhitelist = null;
            ProjectilesWhitelist?.Clear();
            ProjectilesWhitelist = null;
        }
    }
}