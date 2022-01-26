using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class StariteSpinnerProjectile : YoyoType
    {
        protected override float LifeTimeMultiplier => 10f;
        protected override float TopSpeed => 16.5f;
        protected override float MaxRange => 280f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.scale = 0.9f;
        }

        public override void PostAI()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 15);
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
            if (target.life > 0 && !target.buffImmune[ModContent.BuffType<Buffs.Debuffs.BlueFire>()] && Main.rand.NextBool(12))
            {
                dustAmount *= 2;
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 120);
                if (Main.netMode != NetmodeID.Server)
                {
                    AQSound.Play(SoundType.NPCHit, "inflict_bluefire", target.Center, 0.8f);
                }
            }
            for (int i = 0; i < dustAmount; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.MonoSparkleDust>(),
                    Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)) * (4f + Main.rand.NextFloat() * 4f), 150, new Color(150, 170, 200, 100)).noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() / 2f;
            Vector2 drawPos = projectile.Center - Main.screenPosition;
            float mult = 1f / ProjectileID.Sets.TrailCacheLength[projectile.type];
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(10, 10, 100, 0) * (mult * (ProjectileID.Sets.TrailCacheLength[projectile.type] - i)), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
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

            for (float f = 0f; f < 1f; f += 0.25f)
            {
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy((f + time) * (MathHelper.Pi * 2f)) * globalTimeWrappedHourly2, null, new Color(20, 20, 80, 50), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            for (float f = 0f; f < 1f; f += 0.34f)
            {
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy((f + time) * (MathHelper.Pi * 2f)) * globalTimeWrappedHourly2, null, new Color(50, 50, 180, 127), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, drawPos, null, new Color(255, 255, 255, 255), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0f, 0.5f), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}