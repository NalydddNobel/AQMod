using Aequus;
using Aequus.Content;
using Aequus.Particles.Dusts;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.NPCs.RedSprite.Projectiles {
    public class RedSpriteCloudLightning : ModProjectile {
        public override void SetStaticDefaults() {
            AequusProjectile.InflictsHeatDamage.Add(Type);
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;

            //Projectile.GetGlobalProjectile<AQProjectile>().SetupTemperatureStats(20);
        }

        public override Color? GetAlpha(Color drawColor) {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var orig = texture.Size() / 2f;
            var drawPosition = Projectile.Center;
            float speedX = Projectile.velocity.X.Abs();
            lightColor = Projectile.GetAlpha(lightColor);
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            var origin = frame.Size() / 2f;
            RedSprite.DrawWithAura(Main.spriteBatch, texture, drawPosition - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, auraIntensity: -4f);
            return false;
        }

        public override void Kill(int timeLeft) {
            var center = Projectile.Center;
            for (int i = 0; i < 50; i++) {
                int d = Dust.NewDust(Projectile.position, 16, 16, ModContent.DustType<RedSpriteDust>());
                Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
            }
        }
    }
}