using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class UltimateSword : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 0;
            projectile.height = 0;
            projectile.aiStyle = -1;
            projectile.friendly = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
                projectile.Kill();
            if (projectile.width == 0 && projectile.height == 0)
            {
                var item = new Item();
                item.SetDefaults((int)projectile.ai[0]);
                projectile.width = item.width;
                projectile.height = item.height;
                projectile.position = new Vector2(projectile.position.X - projectile.width / 2f, projectile.position.Y - projectile.height / 2f);
            }
            projectile.velocity.X *= 0.98f;
            projectile.velocity.Y += 0.35f;
            projectile.rotation += MathHelper.Clamp(projectile.velocity.Length() * 0.0157f, 0.0728f, 0.157f);
        }

        public override void PostAI()
        {
            if (Main.rand.NextBool())
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<UltimaDust>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.itemTexture[(int)projectile.ai[0]];
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, lightColor, projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(AQUtils.GetTexture<Items.Weapons.Melee.UltimateSword>("_Glow"), projectile.Center - Main.screenPosition, frame, new Color(250, 250, 250, 0), projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Length() > 6f)
            {
                projectile.velocity = oldVelocity * -0.4f;
                projectile.velocity.X += Main.rand.NextFloat(-1f, 1f);
                return false;
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, projectile.position);
            Item.NewItem(projectile.getRect(), (int)projectile.ai[0]);
            var dust = ModContent.DustType<UltimaDust>();
            for (int i = 0; i < 50; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, dust);
            }
        }
    }
}