using Aequus.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.SporeSac {
    public class SporeSacSentry : ModProjectile {
        public static void Convert(Projectile projectile, AequusProjectile aequusProjectile) {
            if (projectile.type == ProjectileID.SporeTrap) {
                aequusProjectile.transform = ModContent.ProjectileType<SporeSacSentry>();
            }
            else if (projectile.type == ProjectileID.SporeTrap2) {
                aequusProjectile.transform = ModContent.ProjectileType<SporeSacSentry2>();
            }
        }

        protected Vector3 _lightColor;

        protected virtual int SporeProjectileID => ProjectileID.SporeTrap;

        public override string Texture => Aequus.ProjectileTexture(SporeProjectileID);

        public override void SetStaticDefaults() {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.CloneDefaults(SporeProjectileID);
            Projectile.aiStyle = -1;
            _lightColor = new(0.2f, 0.275f, 0.075f);
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha);
        }

        public override void AI() {
            float lightingStrength = Projectile.Opacity * Projectile.scale;
            Lighting.AddLight(Projectile.Center, _lightColor.X * lightingStrength, _lightColor.Y * lightingStrength, _lightColor.Z * lightingStrength);

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 90f) {
                Projectile.localAI[0] *= -1f;
            }
            if (Projectile.localAI[0] >= 0f) {
                Projectile.scale += 0.003f;
            }
            else {
                Projectile.scale -= 0.003f;
            }

            Projectile.rotation += 0.0025f * Projectile.scale;
            float wobbleY = 1f;
            float wobbleX = 1f;
            int wobbleID = Projectile.identity % 6;
            switch (wobbleID) {
                case 0:
                    wobbleX *= -1f;
                    break;
                case 1:
                    wobbleY *= -1f;
                    break;
                case 2:
                    wobbleX *= -1f;
                    wobbleY *= -1f;
                    break;
                case 3:
                    wobbleX = 0f;
                    break;
                case 4:
                    wobbleY = 0f;
                    break;
            }

            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 60f) {
                Projectile.localAI[1] = -180f;
            }
            if (Projectile.localAI[1] >= -60f) {
                Projectile.velocity.X += 0.002f * wobbleX;
                Projectile.velocity.Y += 0.002f * wobbleY;
            }
            else {
                Projectile.velocity.X -= 0.002f * wobbleX;
                Projectile.velocity.Y -= 0.002f * wobbleY;
            }
            Projectile.ai[0] += 1f;

            if (Projectile.ai[0] > 5400f * Projectile.MaxUpdates) {
                Projectile.damage = 0;
                Projectile.ai[1] = 1f;
                if (Projectile.alpha < 255) {
                    Projectile.alpha += 5;
                    if (Projectile.alpha > 255) {
                        Projectile.alpha = 255;
                    }
                }
                else if (Projectile.owner == Main.myPlayer) {
                    Projectile.Kill();
                }
            }
            else {
                var ownerDifference = Projectile.Center;
                int sourceProj = Projectile.Aequus().sourceProjIdentity;
                var owner = Projectile.GetHereditaryOwner(sourceProj)?.GetHereditaryOwnerPlayer();
                float ownerDistance = 0f;
                if (owner == null) {
                    ownerDistance += 100f;
                }
                else {
                    ownerDifference -= owner.Center;
                    ownerDistance = ownerDifference.Length() / 100f;
                    if (ownerDistance > 4f) {
                        ownerDistance *= 1.1f;
                    }
                    if (ownerDistance > 5f) {
                        ownerDistance *= 1.2f;
                    }
                    if (ownerDistance > 6f) {
                        ownerDistance *= 1.3f;
                    }
                    if (ownerDistance > 7f) {
                        ownerDistance *= 1.4f;
                    }
                    if (ownerDistance > 8f) {
                        ownerDistance *= 1.5f;
                    }
                    if (ownerDistance > 9f) {
                        ownerDistance *= 1.6f;
                    }
                    if (ownerDistance > 10f) {
                        ownerDistance *= 1.7f;
                    }
                    if (!owner.sporeSac) {
                        ownerDistance += 100f;
                    }
                    if (owner.ownedProjectileCounts[Type] > 5) {
                        ownerDistance += 100f;
                    }
                }
                Projectile.ai[0] += ownerDistance;
                if (Projectile.alpha > 50) {
                    Projectile.alpha -= 10;
                    if (Projectile.alpha < 50) {
                        Projectile.alpha = 50;
                    }
                }
            }

            var targetPoint = default(Vector2);
            float minimumDistance = 340f;
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (!Main.npc[i].CanBeChasedBy(this)) {
                    continue;
                }

                float targetX = Main.npc[i].position.X + Main.npc[i].width / 2;
                float targetY = Main.npc[i].position.Y + Main.npc[i].height / 2;
                float targetDistance = Math.Abs(Projectile.position.X + Projectile.width / 2 - targetX) + Math.Abs(Projectile.position.Y + Projectile.height / 2 - targetY);
                if (targetDistance < minimumDistance) {
                    minimumDistance = targetDistance;
                    targetPoint = Main.npc[i].Center;
                }
            }
            if (targetPoint != default) {
                Vector2 velocityAdd = Vector2.Normalize(targetPoint - Projectile.Center) * 4f;
                Projectile.velocity = (Projectile.velocity * 40f + velocityAdd) / 41f;
            }
            else if (Projectile.velocity.Length() > 0.2) {
                Projectile.velocity *= 0.98f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 11));

            if (Projectile.owner == Main.myPlayer) {
                Vector2 velocity = Vector2.Normalize(new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101))) * 0.3f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, Main.rand.Next(ProjectileID.SporeGas, ProjectileID.SporeGas3 + 1), Projectile.damage, 0f, Projectile.owner);
            }
        }
    }
}