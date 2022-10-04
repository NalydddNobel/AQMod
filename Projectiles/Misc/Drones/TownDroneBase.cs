using Aequus.Content.DronePylons;
using Aequus.Tiles;
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

        public PylonDronePoint PylonManager => DroneWorld.Drones[pylonSpot];

        protected float SpawnInOpacity => spawnInAnimation == 0 ? 1f : -spawnInAnimation / 60f;

        public virtual int ItemDrop => 0;

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
                foreach (var p in DroneWorld.Drones.Keys)
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
                    Projectile.netUpdate = true;
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
            if (pylonSpot == Point.Zero || !DroneWorld.ValidSpot(pylonSpot.X, pylonSpot.Y))
            {
                Projectile.localAI[0] = 0f;
                Projectile.Kill();
                return;
            }
            if (!DroneWorld.Drones.TryGetValue(pylonSpot, out var drone))
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
        }

        public Color GetPylonColor()
        {
            if (AequusTile.PylonColors.TryGetValue(new Point(Main.tile[pylonSpot].TileType, Main.tile[pylonSpot].TileFrameX / 54), out var clr))
                return clr;

            return Color.White;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)Projectile.localAI[0]);
            writer.Write(pylonSpot.X);
            writer.Write(pylonSpot.Y);
            writer.Write(gotoVelocityX);
            writer.Write(gotoVelocityY);
            writer.Write(gotoVelocityXResetTimer);
            writer.Write(gotoVelocityYResetTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadInt32();
            pylonSpot.X = reader.ReadInt32();
            pylonSpot.Y = reader.ReadInt32();
            gotoVelocityX = reader.ReadSingle();
            gotoVelocityY = reader.ReadSingle();
            gotoVelocityXResetTimer = reader.ReadInt32();
            gotoVelocityYResetTimer = reader.ReadInt32();
        }

        public override void Kill(int timeLeft)
        {
            CheckDead();
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

            if (Main.myPlayer == Projectile.owner && Main.rand.NextFloat() < 0.8f)
            {
                int i = Item.NewItem(Projectile.GetSource_Death(), Projectile.getRect(), ItemDrop);
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
                }
            }
        }
    }
}