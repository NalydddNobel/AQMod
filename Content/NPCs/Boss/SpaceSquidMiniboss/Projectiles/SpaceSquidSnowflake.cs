using Aequus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Content.NPCs.Boss.SpaceSquidMiniboss.Projectiles {
    public class SpaceSquidSnowflake : ModProjectile {
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 2;
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.coldDamage = true;

            //Projectile.GetGlobalProjectile<AQProjectile>().SetupTemperatureStats(-40);
        }

        public override void AI() {
            Projectile.rotation += 0.157f;
            if ((int)Projectile.localAI[0] == 0) {
                Projectile.localAI[0] = 1f;
                Projectile.frame = Main.rand.Next(2);
            }
            //if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(8))
            //{
            //    var spawnPosition = new Vector2(Projectile.position.X + Main.rand.NextFloat(Projectile.width), Projectile.position.Y + Main.rand.NextFloat(Projectile.height));
            //    AQMod.Particles.PostDrawPlayers.AddParticle(new BrightSparkle(spawnPosition, Projectile.velocity * 0.1f, new Color(100, 100, 255, 200), Main.rand.NextFloat(0.9f, 1.5f)));
            //}
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var drawPos = Projectile.Center - Main.screenPosition;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Main.spriteBatch.Draw(texture, drawPos, frame, new Color(255, 255, 255, 128), Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            var spotlight = AequusTextures.Bloom0;
            var bloomAura = spotlight.Size() / 2f;
            Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(128, 128, 128, 128), Projectile.rotation, bloomAura, Projectile.scale * 0.22f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(30, 30, 30, 0), Projectile.rotation, bloomAura, Projectile.scale * 0.55f, SpriteEffects.None, 0f);
            return false;
        }
    }
}