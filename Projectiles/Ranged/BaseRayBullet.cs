using Aequus.Common.Configuration;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public class BaseRayBullet : ModProjectile
    {
        public override string Texture => Aequus.TextureNone;

        public virtual Color GetColor()
        {
            if (baseProj != null)
            {
                if (baseProj.type == ProjectileID.PartyBullet)
                {
                    return Main.DiscoColor;
                }
                if (ProjSets.RaygunColors.TryGetValue(baseProj.type, out var color))
                {
                    return color;
                }
            }
            return new Color(255, 255, 255, 255);
        }

        public Projectile baseProj;
        public int trailTimer;

        public void SetBullet(int type)
        {
            baseProj = new Projectile();
            baseProj.SetDefaults(type);
            SetBullet();
        }
        public void SetBullet()
        {
            if (baseProj != null)
            {
                //Projectile.CloneDefaults(baseProj.type);
                Projectile.width = baseProj.width;
                Projectile.height = baseProj.height;
                Projectile.DamageType = baseProj.DamageType;
                Projectile.aiStyle = baseProj.aiStyle;
                Projectile.penetrate = baseProj.penetrate;
                Projectile.usesLocalNPCImmunity = baseProj.usesLocalNPCImmunity;
                Projectile.localNPCHitCooldown = baseProj.localNPCHitCooldown;
                Projectile.usesIDStaticNPCImmunity = baseProj.usesIDStaticNPCImmunity;
                Projectile.idStaticNPCHitCooldown = baseProj.idStaticNPCHitCooldown;
                Projectile.scale = baseProj.scale;
                Projectile.ignoreWater = baseProj.ignoreWater;
                Projectile.tileCollide = baseProj.tileCollide;
                Projectile.extraUpdates = baseProj.extraUpdates;

                AIType = baseProj.type;
            }
            else
            {
                Projectile.aiStyle = -1;
                Projectile.DamageType = DamageClass.Ranged;
            }
            SetDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.width = Math.Max(Projectile.width, 12);
            Projectile.height = Math.Max(Projectile.height, 12);
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, 180);
            Projectile.friendly = true;
            Projectile.extraUpdates++;
            Projectile.extraUpdates *= 6;
            Projectile.hide = true;
            trailTimer = 7;
        }

        public override void AI()
        {
            if (AIType == ProjectileID.ChlorophyteBullet)
            {
                Projectile.alpha = 255;
            }
            if (trailTimer <= 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    int p = Projectile.NewProjectile(new EntitySource_Parent(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RayTrailEffect>(), 0, 0f, Projectile.owner);
                    Main.projectile[p].rotation = Projectile.velocity.ToRotation();
                    ((RayTrailEffect)Main.projectile[p].ModProjectile).color = GetColor().UseA(0);
                }
                trailTimer = 6;
            }
            trailTimer--;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 4;
            height = 4;
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (baseProj != null)
            {
                var value = baseProj.ModProjectile?.OnTileCollide(oldVelocity);
                if (baseProj.type == ProjectileID.MeteorShot || baseProj.type == ProjectileID.NanoBullet)
                {
                    if (Projectile.velocity.X != oldVelocity.X)
                    {
                        Projectile.velocity.X = -oldVelocity.X;
                    }
                    if (Projectile.velocity.Y != oldVelocity.Y)
                    {
                        Projectile.velocity.Y = -oldVelocity.Y;
                    }
                    if (baseProj.type == ProjectileID.MeteorShot)
                    {
                        Projectile.penetrate--;
                        if (Projectile.penetrate == 0)
                        {
                            Projectile.Kill();
                        }
                    }
                    else
                    {
                        Projectile.ai[1]++;
                        if ((int)Projectile.ai[1] == 1)
                        {
                            Projectile.damage = (int)(Projectile.damage * 0.66f);
                        }
                        if (Projectile.ai[1] >= 2f)
                        {
                            Projectile.Kill();
                        }
                        int target = Projectile.FindTargetWithLineOfSight();
                        if (target != -1)
                        {
                            NPC nPC2 = Main.npc[target];
                            Projectile.Distance(nPC2.Center);
                            Projectile.velocity = Projectile.DirectionTo(nPC2.Center).SafeNormalize(-Vector2.UnitY) * Projectile.velocity.Length();
                            Projectile.netUpdate = true;
                        }
                    }
                    trailTimer = 6;
                    return value.GetValueOrDefault(false);
                }
                return value.GetValueOrDefault(true);
            }
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            baseProj?.ModProjectile?.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (baseProj != null)
            {
                if (baseProj.type == ProjectileID.CursedBullet)
                {
                    target.AddBuff(BuffID.CursedInferno, 240);
                }
                else if (baseProj.type == ProjectileID.IchorBullet)
                {
                    target.AddBuff(BuffID.Ichor, 240);
                }
                else if (baseProj.type == ProjectileID.VenomBullet)
                {
                    target.AddBuff(BuffID.Venom, 240);
                }
                else if (baseProj.type == ProjectileID.NanoBullet)
                {
                    target.AddBuff(BuffID.Confused, 1200);
                }
                else if (baseProj.type == ProjectileID.GoldenBullet)
                {
                    target.AddBuff(BuffID.Midas, 1200);
                }
                if (baseProj.penetrate > 1)
                {
                    SpawnExplosion();
                }
            }
            baseProj?.ModProjectile?.OnHitNPC(target, damage, knockback, crit);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(trailTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            trailTimer = reader.ReadInt32();
        }

        public override void Kill(int timeLeft)
        {
            SpawnExplosion();
        }

        private void SpawnExplosion()
        {
            var center = Projectile.Center;
            if (Main.netMode != NetmodeID.Server)
            {
                int amt = (int)(75 * ClientConfiguration.Instance.effectQuality);
                var color = GetColor().UseA(0);
                for (int i = 0; i < amt; i++)
                {
                    float scale = Main.rand.NextFloat(1f, 3f);
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale);
                    var r = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                    d.position = center + r * Main.rand.NextFloat(24f);
                    float speed = Main.rand.NextFloat(9f, 14f);
                    d.velocity = r * (speed - Math.Min(scale * 4f, speed - 0.1f));
                }
            }
            if (baseProj != null)
            {
                if (baseProj.type == ProjectileID.CrystalBullet)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        var r = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                        var explosionPos = Projectile.Center + r * Main.rand.NextFloat(16f, 60f);
                        if (Main.netMode != NetmodeID.Server)
                        {
                            int amt = (int)(35 * ClientConfiguration.Instance.effectQuality);
                            var color = GetColor().UseA(0) * 0.8f;
                            for (int j = 0; j < amt; j++)
                            {
                                float scale = Main.rand.NextFloat(1f, 3f);
                                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale * 0.75f);
                                var r2 = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                                d.position = explosionPos + r2 * Main.rand.NextFloat(6f);
                                float speed = Main.rand.NextFloat(9f, 14f);
                                d.velocity = r2 * (speed - Math.Min(scale * 4f, speed - 0.1f)) * 1.2f;
                            }
                        }
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile.NewProjectile(new EntitySource_Parent(Projectile), explosionPos, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<RayExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            Projectile.NewProjectile(new EntitySource_Parent(Projectile), explosionPos, r * 8f, ProjectileID.CrystalShard, Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
                else if (baseProj.type == ProjectileID.PartyBullet)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f)
                        {
                            var r = f.ToRotationVector2();
                            Projectile.NewProjectile(new EntitySource_Parent(Projectile), center + r * 50f, r * 8f, ProjectileID.ConfettiGun, 0, 0f, Projectile.owner);
                        }
                    }
                }
                else if (baseProj.type == ProjectileID.ExplosiveBullet)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        int amt = (int)(175 * ClientConfiguration.Instance.effectQuality);
                        var color = GetColor().UseA(0) * 1.2f;
                        for (int j = 0; j < amt; j++)
                        {
                            float scale = Main.rand.NextFloat(0.6f, 2.5f);
                            if (Main.rand.NextBool(4))
                            {
                                scale *= 1.5f;
                            }
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale);
                            var r2 = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                            d.position = center + r2 * Main.rand.NextFloat(50f);
                            float speed = Main.rand.NextFloat(9f, 14f);
                            d.velocity = r2 * (speed - Math.Min(scale * 4f, speed - 0.01f)) * 2.15f;
                        }
                    }
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f)
                        {
                            var r = f.ToRotationVector2();
                            var explosionPos = Projectile.Center + r * Main.rand.NextFloat(42f, 68f);
                            Projectile.NewProjectile(new EntitySource_Parent(Projectile), explosionPos, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<RayExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
            }
            if (Main.myPlayer == Projectile.owner)
            {
                // A small bit of velocity is given to this explosion projectile to make it knockback enemies in the correct direction
                // I could just override the modify hit methods and manually apply direction there but blah
                Projectile.NewProjectile(new EntitySource_Parent(Projectile), Projectile.Center, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<RayExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}