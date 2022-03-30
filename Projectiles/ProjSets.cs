using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles
{
    public sealed class ProjSets : ILoadable
    {
        public static Dictionary<int, Color> RaygunColors { get; private set; }
        public static Dictionary<int, int> RaygunConversions { get; private set; }

        public void Load(Mod mod)
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
        }

        public void Unload()
        {
            RaygunColors?.Clear();
            RaygunColors = null;
            RaygunConversions?.Clear();
            RaygunConversions = null;
        }
    }
}