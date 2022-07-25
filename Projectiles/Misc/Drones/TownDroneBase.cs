using Aequus.Content.DronePylons;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Drones
{
    public abstract class TownDroneBase : ModProjectile
    {
        public Point pylonSpot;
        public int spawnInAnimation;

        public float gotoVelocityX;
        public float gotoVelocityY;
        public int gotoVelocityXResetTimer;
        public int gotoVelocityYResetTimer;

        public DronePylonManager PylonManager => DroneSystem.Drones[pylonSpot];

        protected float SpawnInOpacity => spawnInAnimation == 0 ? 1f : -spawnInAnimation / 60f;

        public override void AI()
        {
            if (spawnInAnimation < 0)
            {
                spawnInAnimation--;
                if (spawnInAnimation < -60)
                {
                    spawnInAnimation = 0;
                }
            }
            if (pylonSpot == Point.Zero)
            {
                float closestPylon = 1000f;
                foreach (var p in DroneSystem.Drones.Keys)
                {
                    float d = Vector2.Distance(Projectile.Center, p.ToWorldCoordinates());
                    if (d < closestPylon)
                    {
                        closestPylon = d;
                        pylonSpot = p;
                    }
                }
                if (pylonSpot == Point.Zero)
                {
                    Projectile.Kill();
                    return;
                }
                var townNPCs = PylonManager.NearbyTownNPCs;
                int div = townNPCs.Count;
                foreach (var n in townNPCs)
                {
                    Projectile.damage += n.damage;
                }
                if (div != 0)
                    Projectile.damage /= div;
            }
            if (pylonSpot == Point.Zero || !DroneSystem.ValidSpot(pylonSpot.X, pylonSpot.Y))
            {
                Projectile.localAI[0] = 0f;
                Projectile.Kill();
                return;
            }
            if (!DroneSystem.Drones.TryGetValue(pylonSpot, out var drone))
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
                case 1:
                    return new Color(200, 255, 65, 255);
                case 2:
                    return Color.HotPink * 1.5f;
                case 3:
                    return new Color(230, 165, 255, 255);
                case 4:
                    return Color.SkyBlue * 1.125f;
                case 5:
                    return new Color(255, 222, 120, 255);
                case 6:
                    return new Color(120, 222, 255, 255);
                case 7:
                    return new Color(100, 128, 255, 255);
                case 8:
                    return Color.FloralWhite;
            }

            return Color.White;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(pylonSpot.X);
            writer.Write(pylonSpot.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            pylonSpot.X = reader.ReadInt32();
            pylonSpot.Y = reader.ReadInt32();
        }

        public void CheckDead()
        {
            if ((int)Projectile.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                OnDeath();
            }
        }

        public virtual void OnDeath()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            SoundEngine.PlaySound(SoundID.NPCDeath43.WithVolume(0.25f), Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
                d.noGravity = true;
            }
        }
    }
}