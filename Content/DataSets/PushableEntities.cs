using Aequus;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DataSets {
    public class PushableEntities : IAddRecipes {
        public static HashSet<int> ProjectileIDs { get; private set; }

        void ILoadable.Load(Mod mod) {
            ProjectileIDs = new HashSet<int>();
        }

        public static void AddProj(int type) {
            ProjectileIDs.Add(type);
        }

        void IAddRecipes.AddRecipes(Aequus aequus) {
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

            //NPCIDs.Remove(NPCID.BloodSquid);
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
            ProjectileIDs?.Clear();
            ProjectileIDs = null;
        }
    }
}