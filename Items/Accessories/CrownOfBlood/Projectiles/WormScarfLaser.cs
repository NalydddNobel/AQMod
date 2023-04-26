using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.CrownOfBlood.Projectiles {
    public class WormScarfLaser : ModProjectile {
        public override string Texture => Aequus.VanillaTexture + "Projectile_" + ProjectileID.PurpleLaser;

        public override void SetStaticDefaults() {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 50;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 255, 255, 50);
        }

        public override void AI() {
            if ((int)Projectile.localAI[0] == 0) {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item91.WithVolume(0.8f).WithPitch(-0.1f));
            }
            Lighting.AddLight(Projectile.position, Color.Violet.ToVector3() * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.5f, 0.7f));
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            width = 2;
            height = 2;
            return true;
        }

        public override void Kill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Item10);
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
        }
    }
}