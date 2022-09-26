using Aequus.Buffs.Pets;
using Aequus.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Pets
{
    public class TorraPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            Main.projPet[Projectile.type] = true;
            this.SetTrail(12);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyEater);
            AIType = ProjectileID.BabyEater;
            Projectile.width = 32;
            Projectile.height = 32;
        }

        public override bool PreAI()
        {
            Main.player[Projectile.owner].eater = false;
            if (!AequusHelpers.UpdateProjActive<TorraBuff>(Projectile))
            {
                return false;
            }

            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] % 2 == 0)
                return true;

            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X.UnNaN());
            return false;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X.UnNaN());
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var drawCoords = Projectile.Center;
            Main.instance.LoadProjectile(ProjectileID.Blizzard);
            var texture = TextureAssets.Projectile[ProjectileID.Blizzard].Value;
            var arr = AequusHelpers.CircularVector(6, Main.GameUpdateCount * 0.05f);
            for (int i = 0; i < arr.Length; i++)
            {
                var v = arr[i];
                var frame = texture.Frame(verticalFrames: 5, frameY: (Projectile.identity + i) % 5);
                Main.spriteBatch.Draw(texture, drawCoords + v * 80f * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f + i * MathHelper.Pi / 3f, 0.8f, 1f) * Projectile.scale - Main.screenPosition, frame, AequusHelpers.GetColor(drawCoords + v * 60f * Projectile.scale), v.ToRotation() - MathHelper.PiOver2, new Vector2(frame.Width / 2f, frame.Height - 6), Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}