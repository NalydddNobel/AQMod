using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public sealed class RayTrailEffect : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PrincessWeapon;

        public Color color;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 24;
            color = Color.White;
            color.A = 0;
        }

        public override void AI()
        {
            Projectile.scale += 0.0175f;
            Projectile.alpha += 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = 1f - Projectile.alpha / 255f;
            var texture = TextureAssets.Projectile[Type];
            var origin = texture.Size() / 2f;
            var scale = new Vector2(Projectile.scale * 0.11f, Projectile.scale * 0.245f);
            Main.spriteBatch.Draw(texture.Value, Projectile.Center - Main.screenPosition, null, color * opacity, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
            writer.Write(color.A);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            color.R = reader.ReadByte();
            color.G = reader.ReadByte();
            color.B = reader.ReadByte();
            color.A = reader.ReadByte();
        }
    }
}