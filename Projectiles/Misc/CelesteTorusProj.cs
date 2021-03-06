using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Sentry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class CelesteTorusProj : ModProjectile
    {
        public Vector3 rotation;
        public Vector3 rotation2;
        public bool show2ndRing;
        public float currentRadius;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.netImportant = true;
            Projectile.ignoreWater = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override bool? CanCutTiles() => false;

        public override void AI()
        {
            int projIdentity = (int)Projectile.ai[0] - 1;
            AequusPlayer aequus;
            if (projIdentity > -1)
            {
                projIdentity = AequusHelpers.FindProjectileIdentity(Projectile.owner, projIdentity);
                if (projIdentity == -1 || !Main.projectile[projIdentity].active || !Main.projectile[projIdentity].TryGetGlobalProjectile<SantankSentryProjectile>(out var value))
                {
                    Projectile.Kill();
                    return;
                }

                aequus = value.dummyPlayer?.GetModPlayer<AequusPlayer>();
                Projectile.Center = Main.projectile[projIdentity].Center;
            }
            else
            {
                aequus = Main.player[Projectile.owner].GetModPlayer<AequusPlayer>();
                Projectile.Center = Main.player[Projectile.owner].Center;
            }
            Projectile.scale = 1f;

            var player = Main.player[Projectile.owner];
            if (!player.active || player.dead || (aequus?.celesteTorusItem) == null)
            {
                return;
            }
            Projectile.timeLeft = 2;

            if (Projectile.active)
            {
                int damage = player.GetWeaponDamage(aequus.celesteTorusItem);
                if (Projectile.damage != damage)
                {
                    if (Projectile.damage < damage)
                    {
                        Projectile.damage = Math.Min(Projectile.damage + 2, damage);
                    }
                    else
                    {
                        Projectile.damage = Math.Max(Projectile.damage - 10, damage);
                    }
                }

                float playerPercent = player.statLife / (float)player.statLifeMax2;
                float gotoRadius = Math.Min((int)((float)Math.Sqrt(player.width * player.height) + 20f + player.wingTimeMax * 0.15f + player.wingTime * 0.15f + (1f - playerPercent) * 90f + player.statDefense), 600);
                currentRadius = MathHelper.Lerp(currentRadius, gotoRadius, 0.1f);
                Projectile.scale *= 0.8f + 0.2f * currentRadius / 100f;

                var center = Projectile.Center;
                bool danger = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile) && Vector2.Distance(Main.npc[i].Center, center) < 2000f)
                    {
                        danger = true;
                        break;
                    }
                }

                if (danger)
                {
                    rotation.X = rotation.X.AngleLerp(0f, 0.01f);
                    rotation.Y = rotation.Y.AngleLerp(0f, 0.0075f);
                    rotation.Z += 0.04f + (1f - playerPercent) * 0.0314f;

                    rotation2.X = rotation.X.AngleLerp(0f, 0.01f);
                    rotation2.Y = rotation.Y.AngleLerp(0f, 0.0075f);
                    rotation2.Z += 0.04f + (1f - playerPercent) * 0.0314f;
                }
                else
                {
                    rotation.X += 0.0157f;
                    rotation.Y += 0.01f;
                    rotation.Z += 0.0314f;

                    if (aequus.ExpertBoost)
                    {
                        rotation2.X += 0.0157f;
                        rotation2.Y += 0.0314f;
                        rotation2.Z += 0.011f;
                    }
                }

                show2ndRing = aequus.ExpertBoost;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < 5; i++)
            {
                var pos = GetRot(i, rotation, currentRadius);
                var collisionCenter = projHitbox.Center.ToVector2() + new Vector2(pos.X, pos.Y);
                var collisionRectangle = Utils.CenteredRectangle(collisionCenter, new Vector2(Projectile.width, Projectile.width));
                if (collisionRectangle.Intersects(targetHitbox))
                    return true;
            }
            if (show2ndRing)
            {
                for (int i = 0; i < 8; i++)
                {
                    var pos = GetRot(i, rotation, currentRadius * 2f, 8);
                    var collisionCenter = projHitbox.Center.ToVector2() + new Vector2(pos.X, pos.Y);
                    var collisionRectangle = Utils.CenteredRectangle(collisionCenter, new Vector2(Projectile.width, Projectile.width) * 1.2f);
                    if (collisionRectangle.Intersects(targetHitbox))
                        return true;
                }
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            hitDirection = target.position.X < Main.player[Projectile.owner].position.X ? -1 : 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(rotation.X);
            writer.Write(rotation.Y);
            writer.Write(rotation.Z);
            writer.Write(currentRadius);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotation.X = reader.ReadSingle();
            rotation.Y = reader.ReadSingle();
            rotation.Z = reader.ReadSingle();
            currentRadius = reader.ReadSingle();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            CelesteTorus.RenderPoints.Add(new CelesteTorus.RenderData()
            {
                Position = Projectile.Center,
                Rotation = rotation,
                Radius = currentRadius,
                Scale = Projectile.scale,
                Dye = Main.player[Projectile.owner].Aequus().cCelesteTorus,

                Rotation2 = show2ndRing ? rotation2 : null,
            });
        }

        public static Vector3 GetRot(int i, Vector3 rotation, float currentRadius, int max = 5)
        {
            return Vector3.Transform(new Vector3(currentRadius, 0f, 0f), Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z + MathHelper.TwoPi / max * i));
        }
    }
}