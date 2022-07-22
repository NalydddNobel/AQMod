using Aequus.Content.DronePylons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Drones
{
    public abstract class TownDroneBase : ModProjectile
    {
        public Point pylonSpot;

        public override void AI()
        {
            if (pylonSpot == Point.Zero)
            {
                float closestPylon = 1000f;
                foreach (var p in DronePylonSystem.Drones.Keys)
                {
                    float d = Vector2.Distance(Projectile.Center, p.ToWorldCoordinates());
                    if (d < closestPylon)
                    {
                        closestPylon = d;
                        pylonSpot = p;
                    }
                }
                int div = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].townNPC)
                    {
                        float d = Vector2.Distance(Projectile.Center, Main.npc[i].Center);
                        if (d < 1000f)
                        {
                            Projectile.damage += Main.npc[i].damage;
                        }
                    }
                }
                if (div != 0)
                    Projectile.damage /= div;
            }
            if (pylonSpot == Point.Zero || !DronePylonSystem.ValidSpot(pylonSpot.X, pylonSpot.Y))
            {
                Projectile.localAI[0] = 0f;
                Projectile.Kill();
                return;
            }
            if (!DronePylonSystem.Drones.TryGetValue(pylonSpot, out var drone))
            {
                Projectile.localAI[0] = 1f;
                Projectile.Kill();
                return;
            }
            if (!drone.isActive)
            {
                Projectile.localAI[0] = 2f;
                Projectile.Kill();
                return;
            }
            Projectile.localAI[0] = 3f;
            Projectile.localAI[1] = 0f;
            Projectile.timeLeft = 16;
            if (Main.tile[pylonSpot].TileType == TileID.TeleportationPylon)
            {
                Projectile.localAI[1] = Main.tile[pylonSpot].TileFrameX / 54 + 1;
            }
        }

        public Color GetDrawColor()
        {
            switch ((int)Projectile.localAI[1] - 1) 
            {
                case 0:
                    return new Color(100, 255, 128, 255);
                case 7:
                    return new Color(100, 128, 255, 255);
            }

            return Color.White;
        }
    }
}