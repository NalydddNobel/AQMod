using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres {
    public abstract class SceptreOnHitSparkleBase : ModProjectile {
        public override string Texture => AequusTextures.Flare.Path;
        public abstract Color SparkleColor { get; }

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 16;
        }

        public override Color? GetAlpha(Color lightColor) {
            return SparkleColor;
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var drawColor = Projectile.GetAlpha(lightColor);
            float animation = 1f - Projectile.timeLeft / 16f;
            float rotation = Projectile.rotation;
            if (Projectile.timeLeft % 4 < 2) {
                rotation += MathHelper.PiOver2;
            }
            float scale = MathF.Sin(animation * MathHelper.PiOver2 + MathHelper.PiOver2);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, drawColor * scale, rotation, texture.Size() / 2f, new Vector2(0.5f, 1f) * Projectile.scale * scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * scale, rotation + MathHelper.PiOver2, texture.Size() / 2f, new Vector2(0.3f, 0.5f) * Projectile.scale * scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
