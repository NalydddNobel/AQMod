using AQMod.Assets;
using AQMod.Common.Graphics.Particles;
using AQMod.Content.Dusts;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Effects;
using AQMod.Effects.ScreenEffects;
using AQMod.Effects.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster.GaleStreams
{
    public class SpaceSquidSnowflake : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 360;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;

            projectile.GetGlobalProjectile<AQProjectile>().canHeat = false;
            projectile.GetGlobalProjectile<AQProjectile>().temperature = -20;
        }

        public override void AI()
        {
            projectile.rotation += 0.157f;
            if ((int)projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1f;
                projectile.frame = Main.rand.Next(2);
            }
            if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(10))
            {
                var spawnPosition = new Vector2(projectile.position.X + Main.rand.NextFloat(projectile.width), projectile.position.Y + Main.rand.NextFloat(projectile.height));
                ParticleLayers.AddParticle_PostDrawPlayers(new BrightSparkle(spawnPosition, projectile.velocity * 0.1f, new Color(100, 100, 255, 200), Main.rand.NextFloat(1.2f, 2.5f)));
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var drawPos = projectile.Center - Main.screenPosition;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Main.spriteBatch.Draw(texture, drawPos, frame, new Color(255, 255, 255, 128), projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            var spotlight = AQTextures.Lights[LightTex.Spotlight15x15];
            Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(128, 128, 128, 128), projectile.rotation, spotlight.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            spotlight = AQTextures.Lights[LightTex.Spotlight36x36];
            Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(30, 30, 30, 0), projectile.rotation, spotlight.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}