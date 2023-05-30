using Aequus.Buffs.Debuffs;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee {
    public class CauterizerSlash : ModProjectile
    {
        private bool _didEffects;

        public override void SetStaticDefaults()
        {
            this.SetTrail(15);
            PushableEntities.AddProj(Type);
            AequusProjectile.InflictsHeatDamage.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.timeLeft = 360;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16 * 6;
            Projectile.scale = 0.9f;
            _didEffects = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void AI()
        {
            var r = Utils.CenteredRectangle(Projectile.Center, new Vector2(Projectile.width / 8f, Projectile.height / 8f));
            if (Collision.SolidCollision(r.TopLeft(), r.Width, r.Height))
            {
                if (Projectile.velocity.Length() > 1f)
                {
                    Projectile.velocity.Normalize();
                }
                Projectile.velocity *= 0.99f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.velocity.Length() < 1f)
            {
                Projectile.velocity *= 0.99f;
                Projectile.alpha += 5;
                Projectile.scale -= 0.01f;
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.velocity *= 0.975f;
                if (Main.rand.NextBool(3 + Projectile.alpha / 4))
                {
                    var d = Dust.NewDustPerfect(Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2)) * Main.rand.NextFloat(-66f, 66f) * Projectile.scale, DustID.Torch, Projectile.velocity * Main.rand.NextFloat(0.9f), 0, default, 1.5f);
                    d.noGravity = true;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.5f);
                }
            }

            if (!_didEffects)
            {
                _didEffects = true;
                if (Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            var normal = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + normal * -46f, Projectile.Center + normal * 46f, 32f * Projectile.scale, ref _);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool()) {
                target.AddBuffs(180, 1, CrimsonHellfire.Debuffs);
            }

            // Only give penetration penalty if you're not attacking the Wall of Flesh or one of its minon.
            if (target.type != NPCID.LeechHead && target.type != NPCID.LeechBody && target.type != NPCID.LeechTail && target.type != NPCID.TheHungry && target.type != NPCID.TheHungryII && target.type != NPCID.WallofFlesh && target.type != NPCID.WallofFleshEye) {
                Projectile.damage = (int)(Projectile.damage * 0.8f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            this.SetTrail(15);
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            Main.EntitySpriteDraw(AequusTextures.Bloom0, Projectile.position + offset - Main.screenPosition, null, new Color(255, 40, 20, 50) * Projectile.Opacity * 0.8f, Projectile.rotation, AequusTextures.Bloom0.Size() / 2f, new Vector2(1.5f, 1f) * Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            for (int i = 0; i < trailLength; i++)
            {
                float progress = Helper.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(128, 20, 10, 30) * Projectile.Opacity * progress * 0.5f, Projectile.oldRot[i], origin, Projectile.scale * (1.5f - progress * 0.4f), SpriteEffects.FlipHorizontally, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, new Vector2(Projectile.scale, Projectile.scale * 1.5f), SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}