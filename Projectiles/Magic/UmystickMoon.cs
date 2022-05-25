using Aequus.Graphics.Prims;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class UmystickMoon : ModProjectile
    {
        public static SoundStyle UmysticDestroyd { get; private set; }

        protected PrimRenderer prim;
        protected Color _glowClr;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                UmysticDestroyd = new SoundStyle("Aequus/Sounds/Items/Umystick/destroy", 3) { Volume = 0.6f, };
            }
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.scale = 0.75f;
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1f;
                Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
                Projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                _glowClr = new Color(128, 70, 70, 0);
                if (Projectile.frame == 1)
                    _glowClr = new Color(90, 128, 50, 0);
                else if (Projectile.frame == 2)
                    _glowClr = new Color(70, 70, 128, 0);
            }
            Projectile.rotation += Projectile.velocity.Length() * 0.0157f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            var origin = frame.Size() / 2f;
            var center = Projectile.Center;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            if (prim == null)
            {
                prim = new PrimRenderer(Images.Trail[0].Value, PrimRenderer.DefaultPass,
                    (p) => new Vector2(14f - p * 14f) * Projectile.scale, (p) => _glowClr * (1f - p),
                    drawOffset: Projectile.Size / 2f);
            }
            prim.Draw(Projectile.oldPos);
            Main.spriteBatch.Draw(Images.Bloom[0].Value, center - Main.screenPosition, null, _glowClr, Projectile.rotation, Images.Bloom[0].Value.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Images.Bloom[0].Value, center - Main.screenPosition, null, _glowClr, Projectile.rotation, Images.Bloom[0].Value.Size() / 2f, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, new Color(250, 250, 250, 160), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = Projectile.Center;
            float size = Projectile.width / 2f;
            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(UmysticDestroyd, Projectile.Center);
            }
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>());
                var n = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToRotationVector2();
                Main.dust[d].position = center + n * Main.rand.NextFloat(0f, size);
                Main.dust[d].velocity = n * Main.rand.NextFloat(2f, 7f);
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1.75f);
                Main.dust[d].color = _glowClr * Main.rand.NextFloat(0.8f, 2f);
            }
        }
    }
}