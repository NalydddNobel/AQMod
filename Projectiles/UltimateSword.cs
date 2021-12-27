using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class UltimateSword : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
                Projectile.Kill();
            if (Projectile.width == 0 && Projectile.height == 0)
            {
                var item = new Item();
                item.SetDefaults((int)Projectile.ai[0]);
                Projectile.width = item.width;
                Projectile.height = item.height;
                Projectile.position = new Vector2(Projectile.position.X - Projectile.width / 2f, Projectile.position.Y - Projectile.height / 2f);
            }
            Projectile.velocity.X *= 0.98f;
            Projectile.velocity.Y += 0.35f;
            Projectile.rotation += MathHelper.Clamp(Projectile.velocity.Length() * 0.0157f, 0.0728f, 0.157f);
        }

        public override void PostAI()
        {
            if (Main.rand.NextBool())
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(250, 250, 250, 0), 0.8f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadItem(ModContent.ItemType<Items.Weapons.Melee.UltimateSword>()); 
            var asset = TextureAssets.Item[ModContent.ItemType<Items.Weapons.Melee.UltimateSword>()];
            if (asset.Value != null)
            {
                var texture = asset.Value;
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);

                asset = ModContent.Request<Texture2D>(AQUtils.GetPath<Items.Weapons.Melee.UltimateSword>("_Glow"), AssetRequestMode.AsyncLoad);

                if (asset.Value != null)
                {
                    Main.spriteBatch.Draw(asset.Value, Projectile.Center - Main.screenPosition, frame, new Color(250, 250, 250, 0), Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
                }
            }
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 36;
            height = 36;
            fallThrough = Projectile.position.Y + Projectile.height < Main.player[Projectile.owner].position.Y;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Length() > 6f)
            {
                Projectile.velocity = oldVelocity * -0.4f;
                Projectile.velocity.X += Main.rand.NextFloat(-1f, 1f);
                return false;
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            //AQItem.DropInstancedItem(Projectile.owner, Projectile.getRect(), ModContent.ItemType<Items.Weapons.Melee.UltimateSword>());
            Item.NewItem(Projectile.getRect(), ModContent.ItemType<Items.Weapons.Melee.UltimateSword>());
            var dust = ModContent.DustType<MonoDust>();
            for (int i = 0; i < 50; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dust, 0f, 0f, 0, new Color(250, 250, 250, 0), 0.8f);
            }
        }
    }
}