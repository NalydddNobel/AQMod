using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Elites.Misc {
    public class XenonSpore : ModProjectile {
        public override void SetStaticDefaults() {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 1000;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16 * 6;
            Projectile.alpha = 250;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 255, 255, 200);
        }

        public override void AI() {
            Projectile.rotation += 0.04f;
            if (Main.rand.NextBool(10)) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Flare_Blue, Scale: 1.5f * Projectile.scale * Projectile.Opacity);
                d.noGravity = true;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.5f);
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 300f) {
                Projectile.alpha += 16;
                Projectile.scale -= 0.004f;
                if (Projectile.alpha > 255) {
                    Projectile.Kill();
                }
            }
            else if (Projectile.alpha > 0) {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0) {
                    Projectile.alpha = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);

            Main.EntitySpriteDraw(AequusTextures.Bloom0, Projectile.position + offset - Main.screenPosition, null, new Color(48, 100, 128, 0) * Projectile.Opacity * 0.8f, Projectile.rotation, AequusTextures.Bloom0.Size() / 2f, 0.4f * Projectile.scale, SpriteEffects.FlipHorizontally, 0);

            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.85f, 1.05f), SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}