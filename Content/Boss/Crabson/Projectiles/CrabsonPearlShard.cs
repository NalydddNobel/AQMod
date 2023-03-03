using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.Crabson.Projectiles
{
    public class CrabsonPearlShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80;
            if (Main.getGoodWorld)
            {
                Projectile.timeLeft *= 2;
            }
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.5f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.6f, 1f));
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft < 60 && !Main.getGoodWorld)
            {
                Projectile.tileCollide = true;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 30);
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            var center = Projectile.Center;
            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.Item50.WithVolume(0.5f).WithPitch(-0.1f), Projectile.Center);
            }
            float dustRadius = Projectile.Size.Length() * 0.6f;
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 160, 255, 10));
                Main.dust[d].position = center + new Vector2(Main.rand.NextFloat(dustRadius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].velocity = (Main.dust[d].position - center) * 3f;
            }
            for (int i = 0; i < 2; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 160, 255, 10), Main.rand.NextFloat(1.5f, 2f));
                Main.dust[d].position = center + new Vector2(Main.rand.NextFloat(dustRadius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].velocity = (Main.dust[d].position - center) * 4.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Projectile.type].Value;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
            var origin = texture.Size() / 2f;
            var drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, Projectile.position + offset, null, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset, null, drawColor * 0.3f, Projectile.rotation, origin, Projectile.scale * 2f, SpriteEffects.None, 0);
            return false;
        }
    }
}