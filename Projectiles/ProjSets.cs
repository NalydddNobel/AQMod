using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles
{
    public sealed class ProjSets : ModType
    {
        public static Dictionary<int, Color> RaygunColors { get; private set; }
        public static Dictionary<int, int> RaygunConversions { get; private set; }
        public static HashSet<int> WindUpdates { get; private set; }

        protected sealed override void Register()
        {
        }

        public override void Load()
        {
            RaygunColors = new Dictionary<int, Color>()
            {
                [ProjectileID.Bullet] = new Color(1, 255, 40, 255),
                [ProjectileID.MeteorShot] = new Color(30, 255, 200, 255),
                [ProjectileID.CrystalBullet] = new Color(200, 112, 145, 255),
                [ProjectileID.CursedBullet] = new Color(120, 228, 50, 255),
                [ProjectileID.IchorBullet] = new Color(228, 200, 50, 255),
                [ProjectileID.ChlorophyteBullet] = new Color(135, 255, 120, 255),
                [ProjectileID.BulletHighVelocity] = new Color(255, 255, 235, 255),
                [ProjectileID.VenomBullet] = new Color(128, 30, 255, 255),
                [ProjectileID.NanoBullet] = new Color(60, 200, 255, 255),
                [ProjectileID.ExplosiveBullet] = new Color(255, 120, 60, 255),
                [ProjectileID.GoldenBullet] = new Color(255, 255, 10, 255),
                [ProjectileID.MoonlordBullet] = new Color(60, 215, 245, 255),
            };
            RaygunConversions = new Dictionary<int, int>();
            WindUpdates = new HashSet<int>();
        }

        public override void SetupContent()
        {
            List<int> windAIStyles = new List<int>() 
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
            };

            foreach (var p in ContentSamples.ProjectilesByType)
            {
                if (WindUpdates.Contains(p.Key))
                {
                    continue;
                }
                try
                {
                    var projectile = p.Value;
                    if (windAIStyles.Contains(projectile.aiStyle))
                    {
                        WindUpdates.Add(p.Key);
                    }
                }
                catch (Exception e)
                {
                    var l = Aequus.Instance.Logger;
                    l.Error("An error occured when doing algorithmic checks for sets for {" + Lang.GetProjectileName(p.Key).Value + ", ID: " + p.Key + "}", e);
                }
            }
        }

        public override void Unload()
        {
            WindUpdates?.Clear();
            WindUpdates = null;
            RaygunColors?.Clear();
            RaygunColors = null;
            RaygunConversions?.Clear();
            RaygunConversions = null;
        }
    }
}