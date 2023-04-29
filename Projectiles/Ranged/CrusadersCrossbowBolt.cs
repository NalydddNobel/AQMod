using Aequus.Common.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public class CrusadersCrossbowBolt : ModProjectile
    {
        public TrailRenderer prim;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Main.teamColor[Main.player[Projectile.owner].team];
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.netMode != NetmodeID.SinglePlayer && Main.myPlayer == Projectile.owner)
            {
                int plr = Helper.FindPlayerWithin(new Rectangle((int)Projectile.position.X - 40, (int)Projectile.position.Y - 40, 80 + Projectile.width, 80 + Projectile.height));
                if (plr != -1 && plr != Projectile.owner)
                {
                    if (Main.player[plr].team == Main.player[Projectile.owner].team)
                    {
                        int healing = Main.DamageVar(Projectile.damage / 2f);
                        Main.player[plr].HealEffect(healing, broadcast: false);
                        Main.player[plr].statLife += healing;
                        if (Main.player[plr].statLife > Main.player[plr].statLifeMax2)
                        {
                            Main.player[plr].statLife = Main.player[plr].statLifeMax2;
                        }
                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, plr, healing);
                        }
                        Projectile.Kill();
                    }
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 2;
            height = 2;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
            var drawColor = Projectile.GetAlpha(lightColor);
            if (prim == null)
            {
                prim = new TrailRenderer(TrailTextures.Trail[0].Value, TrailRenderer.DefaultPass, (p) => new Vector2(6f) * (1f - p), (p) => Projectile.GetAlpha(Color.White).UseA(0) * 0.9f * (float)Math.Pow(1f - p, 2f))
                {
                    drawOffset = Projectile.Size / 2f
                };
            }
            prim.Draw(Projectile.oldPos);
            var frame = Projectile.Frame();
            var origin = new Vector2(frame.Width, frame.Height / 2f - 1f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset, frame, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.Pi, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset, new Rectangle(frame.X, frame.Y + frame.Height, frame.Width, frame.Height), Color.White, Projectile.rotation + MathHelper.Pi, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}