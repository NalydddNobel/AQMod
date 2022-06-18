using Aequus.Buffs.Minion;
using Aequus.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class CorruptPlantMinion : MinionBase
    {
        public const float VineSeparation = 12f;

        public class Vine
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public float Rotation;
        }

        public Vine[] Chains;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        public override void AI()
        {
            Projectile.localAI[0] = CountSlots();
            if (!AequusHelpers.UpdateProjActive<CorruptPlantBuff>(Projectile) || Projectile.localAI[0] <= 0)
            {
                return;
            }

            Projectile.damage = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<CorruptPlantCounter>() && Main.projectile[i].owner == Projectile.owner)
                {
                    if (Main.projectile[i].damage > Projectile.damage)
                    {
                        Projectile.originalDamage = Main.projectile[i].damage;
                    }
                }
            }

            var gotoPosition = DefaultIdlePosition();
            var diff = gotoPosition - Projectile.Center;
            if ((Main.player[Projectile.owner].Center - Projectile.Center).Length() > 2000f)
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity *= 0.1f;
                diff = Vector2.UnitY;
            }
            var ovalDiff = new Vector2(diff.X, diff.Y *= 3f);
            float ovalLength = ovalDiff.Length();
            if (ovalLength > 20f)
            {
                var velocity = diff / 8f;
                if (velocity.Length() < 4f)
                {
                    velocity = Vector2.Normalize(velocity).UnNaN() * 4f;
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocity, 0.08f);
            }

            Projectile.spriteDirection = Main.player[Projectile.owner].direction;
        }
        public int CountSlots()
        {
            return Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<CorruptPlantCounter>()];
        }
        public int Range(int slots)
        {
            return 7 + slots;
        }

        public override Vector2 IdlePosition(Player player, int leader, int minionPos, int count)
        {
            return base.IdlePosition(player, leader, minionPos, count) + new Vector2(0f, -20f + -10f * Math.Min(Range(CountSlots()) / 2f, 10));
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * (1f + CountSlots() * 0.55f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);

            ManageChains();
            var chainTexture = ModContent.Request<Texture2D>(Texture + "_Chain").Value;
            var chainOrigin = chainTexture.Size();
            foreach (var c in Chains)
            {
                Main.EntitySpriteDraw(chainTexture, c.Position - Main.screenPosition, null, Lighting.GetColor(c.Position.ToTileCoordinates()), c.Rotation + MathHelper.PiOver2, chainOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public void ManageChains()
        {
            var plr = Main.player[Projectile.owner].Center;
            int range = Math.Min(Range((int)Projectile.localAI[0]), 10) + 1;
            if (Chains == null)
            {
                Chains = new Vine[range];
                for (int i = 0; i < range; i++)
                {
                    Chains[i] = new Vine()
                    {
                        Position = new Vector2(plr.X, plr.Y + i * -10f),
                        Velocity = Main.rand.NextVector2Unit(),
                    };
                }
            }
            else if (range != Chains.Length)
            {
                Array.Resize(ref Chains, range);
                for (int i = 0; i < range; i++)
                {
                    if (Chains[i] == null)
                    {
                        Chains[i] = new Vine()
                        {
                            Position = new Vector2(plr.X, plr.Y - i * 10f)
                        };
                    }
                }
            }

            if (!Aequus.GameWorldActive)
            {
                return;
            }

            Chains[0].Position = plr;
            Chains[0].Velocity = Vector2.Zero;

            var proj = Projectile.Center;
            Chains[^1].Position = proj;
            Chains[^1].Velocity = Vector2.Zero;

            for (int i = 1; i < Chains.Length; i++)
            {
                var diff = Chains[i - 1].Position - Chains[i].Position;
                var d = diff.Length();
                if (d > VineSeparation)
                {
                    Chains[i].Position += Vector2.Normalize(diff) * (d - VineSeparation);
                    Chains[i].Velocity += Vector2.Normalize(diff) * (d - VineSeparation) * 0.1f;
                    Chains[i - 1].Velocity -= Vector2.Normalize(diff) * (d - VineSeparation) * 0.099f;
                }
                if (d < 2f)
                {
                    Chains[i].Velocity += Main.rand.NextVector2Unit() * 0.1f;
                }

                if (Chains[i].Velocity.Length() > 10f)
                {
                    Chains[i].Velocity = Vector2.Normalize(Chains[i].Velocity) * 10f;
                }
                Chains[i].Position += Chains[i].Velocity;
                Chains[i].Velocity += Main.rand.NextVector2Unit() * 0.05f;

                float p = AequusHelpers.CalcProgress(Chains.Length, i);
                Chains[i].Velocity += (proj - Chains[i].Position) * 0.005f * (1f - p);

                Chains[i].Rotation = (Chains[i].Position - Chains[i - 1].Position).ToRotation();
            }
        }
    }

    public class CorruptPlantCounter : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetStaticDefaults()
        {
            GlowCore.ProjectileBlacklist.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.AbigailCounter);
            Projectile.aiStyle = -1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (!AequusHelpers.UpdateProjActive<CorruptPlantBuff>(Projectile))
            {
                return;
            }
            Projectile.position = Main.player[Projectile.owner].position;
            Projectile.position.X += Projectile.minionPos * (Projectile.width + 4f);
        }
    }
}