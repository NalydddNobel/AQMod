using AQMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon.Equips
{
    public class OmegaStariteAccessory : ModProjectile
    {
        public Vector3 rotation;
        public float currentRadius;

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.hide = true;
            projectile.netImportant = true;
            projectile.ignoreWater = true;
            projectile.manualDirectionChange = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 6;
        }

        public override bool? CanCutTiles() => false;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            AQPlayer aQPlayer = player.GetModPlayer<AQPlayer>();
            if (!player.active || player.dead || aQPlayer.accOmegaStarite == null)
            {
                projectile.active = false;
            }
            else
            {
                projectile.Center = player.Center;
            }

            if (projectile.active)
            {
                int damage = player.GetWeaponDamage(aQPlayer.accOmegaStarite);
                if (projectile.damage != damage)
                {
                    if (projectile.damage < damage)
                    {
                        projectile.damage = Math.Min(projectile.damage + 2, damage);
                    }
                    else
                    {
                        projectile.damage = Math.Max(projectile.damage - 10, damage);
                    }
                }

                float playerPercent = player.statLife / (float)player.statLifeMax2;
                float gotoRadius = Math.Min((int)((float)Math.Sqrt(player.width * player.height) + 20f + player.wingTimeMax * 0.15f + player.wingTime * 0.15f + (1f - playerPercent) * 90f + player.statDefense), 600);
                currentRadius = MathHelper.Lerp(currentRadius, gotoRadius, 0.1f);
                projectile.scale = Math.Min(1f + currentRadius * 0.006f + projectile.damage * 0.009f + projectile.knockBack * 0.0015f, 2.5f);

                var center = player.Center;
                bool danger = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].IsntFriendly() && Vector2.Distance(Main.npc[i].Center, center) < 2000f)
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
                }
                else
                {
                    rotation.X += 0.0157f;
                    rotation.Y += 0.01f;
                    rotation.Z += 0.0314f;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            for (int i = 0; i < AQPlayer.MaxCelesteTorusOrbs; i++)
            {
                var pos = GetRot(i);
                var collisionCenter = player.Center + new Vector2(pos.X, pos.Y);
                if (Utils.CenteredRectangle(collisionCenter, new Vector2(projectile.width, projectile.width)).Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            hitDirection = target.position.X < Main.player[projectile.owner].position.X ? -1 : 1;
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

        public Vector3 GetRot(int i)
        {
            return Vector3.Transform(new Vector3(currentRadius, 0f, 0f), Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z + MathHelper.TwoPi / 5 * i));
        }
    }
}