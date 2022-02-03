using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public sealed class SnowgraveProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 180;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.coldDamage = true;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 4;
        }

        public override void AI()
        {
            if ((int)projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = projectile.width / 6;
            }
            if (projectile.ai[0] > 0f)
                projectile.ai[0]--;
        }

        public override bool CanDamage()
        {
            return projectile.ai[0] <= 0f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            switch (target.type) 
            {
                case NPCID.TheDestroyer:
                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                    {
                        projectile.ai[0] = 50f;
                    }
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var center = projectile.Center - Main.screenPosition;
            var texture = Main.projectileTexture[projectile.type];
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var origin = frame.Size() / 2f;
            spriteBatch.Draw(texture, center, frame, Color.White, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(texture, new Vector2(center.X + projectile.localAI[0] * i, center.Y), frame, Color.White, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, new Vector2(center.X - projectile.localAI[0] * i, center.Y), frame, Color.White, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}