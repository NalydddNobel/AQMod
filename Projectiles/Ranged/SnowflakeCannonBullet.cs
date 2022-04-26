using Aequus.Common.Catalogues;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public sealed class SnowflakeCannonBullet : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.NorthPoleSnowflake;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = Main.projFrames[ProjectileID.NorthPoleSnowflake];

            this.SetTrail(8);

            WindMovementCatalogue.ProjectilesWhitelist.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            if ((int)Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1f;
                Projectile.ai[1] = Projectile.velocity.Length();
                Projectile.frame = Main.rand.Next(Main.projFrames[Type]);
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * Projectile.ai[1];
            Projectile.rotation += Projectile.ai[1] * 0.075f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, crit ? 480 : 240);
            Projectile.ai[0] += 30f;
            for (int i = 0; i < 5; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(15, 120 + Main.rand.Next(40), 222, 0));
                d.velocity = (d.position - Projectile.Center) / 2f;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item51, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(15, 120 + Main.rand.Next(40), 222, 0));
                d.velocity = (d.position - Projectile.Center) / 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            var trailColor = new Color(15, 120, 222, 0) * (1f - Projectile.alpha / 255f);
            var origin = frame.Size() / 2f;
            this.DrawTrail((v, progress) => 
            {
                Main.EntitySpriteDraw(texture, v - Main.screenPosition, frame, Color.Lerp(new Color(15, 120, 222, 0), new Color(1, 111, 255), AequusHelpers.Wave(progress * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 2f, 0f, 1f)) * progress, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            });
            var drawCoordinates = Projectile.Center - Main.screenPosition;
            foreach (var v in AequusHelpers.CircularVector(4))
            {
                Main.EntitySpriteDraw(texture, drawCoordinates + v * 2f, frame, new Color(100, 120, 255, 0), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, drawCoordinates, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}