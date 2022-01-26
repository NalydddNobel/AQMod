using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Effects.Particles;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class ResonanceProjectile : YoyoType
    {
        protected override float LifeTimeMultiplier => 45f;
        protected override float TopSpeed => 6f;
        protected override float MaxRange => 360f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.extraUpdates = 5;
        }

        public override void PostAI()
        {
            if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(3))
            {
                Particle.PreDrawProjectiles.AddParticle(new EmberParticleSubtractColorUsingScale(projectile.getRect().RandomPosition(), new Vector2(0f, 0f), new Color(250, 150, 50, 25), Main.rand.NextFloat(0.8f, 1f)));
            }
            Lighting.AddLight(projectile.Center, new Vector3(1f, 0.7f, 0.1f));
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int dustAmount = damage / 10;
            if (dustAmount < 1)
            {
                dustAmount = 1;
            }
            if (crit)
            {
                dustAmount *= 2;
            }
            if (target.life > 0 && !(target.buffImmune[BuffID.OnFire] && target.buffImmune[ModContent.BuffType<Buffs.Debuffs.BlueFire>()]) && Main.rand.NextBool(8))
            {
                dustAmount *= 2;
                target.AddBuff(BuffID.OnFire, 1200);
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 120);
                if (Main.netMode != NetmodeID.Server)
                {
                    AQSound.Play(SoundType.NPCHit, "inflict_bluefire", target.Center, 0.8f);
                }
            }
            for (int i = 0; i < dustAmount; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.MonoSparkleDust>(),
                    Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)) * (4f + Main.rand.NextFloat() * 4f), 150, new Color(95, 60, 20, 100)).noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            var origin = texture.Size() / 2f;
            float mult = 1f / ProjectileID.Sets.TrailCacheLength[projectile.type];
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(100, 70, 50, 0) * (mult * (ProjectileID.Sets.TrailCacheLength[projectile.type] - i)), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }

            float time = Main.GameUpdateCount / 240f + Main.GlobalTime * 0.04f;
            float globalTimeWrappedHourly2 = Main.GlobalTime;
            globalTimeWrappedHourly2 %= 5f;
            globalTimeWrappedHourly2 /= 2.5f;
            if (globalTimeWrappedHourly2 >= 1f)
            {
                globalTimeWrappedHourly2 = 2f - globalTimeWrappedHourly2;
            }
            globalTimeWrappedHourly2 = globalTimeWrappedHourly2 * 0.5f + 0.5f;

            for (float f = 0f; f <= 1f; f += 0.25f)
            {
                spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition + new Vector2(0f, 8f).RotatedBy((f + time) * (MathHelper.Pi * 2f)) * globalTimeWrappedHourly2, null, new Color(40, 40, 40, 40), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            for (float f = 0f; f < 1f; f += 0.34f)
            {
                spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition + new Vector2(0f, 4f).RotatedBy((f + time) * (MathHelper.Pi * 2f)) * globalTimeWrappedHourly2, null, new Color(100, 70, 70, 100), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }

            var spotlight = AQTextures.Lights[LightTex.Spotlight30x30];
            var spotlightOrigin = spotlight.Size() / 2f;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                float colorMultiplier = mult * (ProjectileID.Sets.TrailCacheLength[projectile.type] - i);
                colorMultiplier *= colorMultiplier;
                Main.spriteBatch.Draw(spotlight, projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(100, 70, 1, 0) * colorMultiplier, 0f, spotlightOrigin, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(spotlight, projectile.position + offset - Main.screenPosition, null, new Color(125, 80, 60, 0), 0f, spotlightOrigin, projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(AQTextures.Lights[LightTex.Spotlight66x66], projectile.position + offset - Main.screenPosition, null, new Color(55, 25, 10, 0), 0f, AQTextures.Lights[LightTex.Spotlight66x66].Size() / 2f, projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 255), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0f, 0.5f), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}