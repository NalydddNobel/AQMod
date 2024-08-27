using Aequus.Common.Graphics.Primitives;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.SpaceSquid.Projectiles {
    public class SpaceSquidLaser : ModProjectile {
        private TrailRenderer prim;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            LegacyPushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.coldDamage = true;

            //Projectile.GetGlobalProjectile<AQProjectile>().SetupTemperatureStats(-40);
        }

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var orig = texture.Size() / 2f;
            var drawPos = Projectile.Center - Main.screenPosition;
            var drawColor = new Color(10, 200, 80, 0);
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            if (prim == null) {
                prim = new TrailRenderer(TrailTextures.Trail[2].Value, TrailRenderer.DefaultPass, (p) => new Vector2(Projectile.width - p * Projectile.width), (p) => drawColor * (1f - p),
                    drawOffset: Projectile.Size / 2f);
            }
            prim.Draw(Projectile.oldPos);
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 0), Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
            var spotlight = AequusTextures.Bloom0;
            Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 0), Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * 0.22f, SpriteEffects.None, 0f);
            return false;
        }
    }
}