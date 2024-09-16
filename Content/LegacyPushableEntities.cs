﻿using System;
using System.Collections.Generic;

namespace Aequus.Content;

public class LegacyPushableEntities : IAddRecipes {
    public static HashSet<int> NPCIDs { get; private set; }
    public static HashSet<int> ProjectileIDs { get; private set; }

    void ILoadable.Load(Mod mod) {
        NPCIDs = new HashSet<int>();
        ProjectileIDs = new HashSet<int>();
    }

    public static void AddProj(int type) {
        ProjectileIDs.Add(type);
    }

    void IAddRecipes.AddRecipes() {
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

        NPCIDs.Remove(NPCID.BloodSquid);
    }
    public void AutomaticEntries_NPCs(HashSet<int> aiStyles) {
        foreach (var n in ContentSamples.NpcsByNetId) {
            if (NPCIDs.Contains(n.Key)) {
                continue;
            }
            try {
                var npc = n.Value;
                if (aiStyles.Contains(npc.aiStyle)) {
                    NPCIDs.Add(n.Key);
                }
            }
            catch (Exception e) {
                Aequus.Instance.Logger.Error("An error occured when doing algorithmic checks for sets for {" + Lang.GetNPCName(n.Key).Value + ", ID: " + n.Key + "}", e);
            }
        }
    }
    public void AutomaticEntries_Projectiles(HashSet<int> aiStyles) {
        foreach (var p in ContentSamples.ProjectilesByType) {
            if (ProjectileIDs.Contains(p.Key)) {
                continue;
            }
            try {
                var projectile = p.Value;
                if (aiStyles.Contains(projectile.aiStyle)) {
                    ProjectileIDs.Add(p.Key);
                }
            }
            catch (Exception e) {
                Aequus.Instance.Logger.Error("An error occured when doing algorithmic checks for sets for {" + Lang.GetProjectileName(p.Key).Value + ", ID: " + p.Key + "}", e);
            }
        }
    }

    void ILoadable.Unload() {
        NPCIDs?.Clear();
        NPCIDs = null;
        ProjectileIDs?.Clear();
        ProjectileIDs = null;
    }

    #region Mod Calls
    [ModCall("PushableProjectile")]
    public static bool IsProjectilePushable(int proj, bool? setValue = true) {
        if (setValue.HasValue) {
            if (setValue.Value) {
                ProjectileIDs.Add(proj);
            }
            else {
                ProjectileIDs.Remove(proj);
            }
        }

        return ProjectileIDs.Contains(proj);
    }

    [ModCall("PushableNPC")]
    public static bool IsNPCPushable(int proj, bool? setValue = true) {
        if (setValue.HasValue) {
            if (setValue.Value) {
                NPCIDs.Add(proj);
            }
            else {
                NPCIDs.Remove(proj);
            }
        }

        return NPCIDs.Contains(proj);
    }
    #endregion
}