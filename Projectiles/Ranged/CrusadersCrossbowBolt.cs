using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public sealed class CrusadersCrossbowBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 8;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.0005f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.netMode != NetmodeID.SinglePlayer && Main.myPlayer == Projectile.owner)
            {
                int plr = AequusHelpers.CheckForPlayers(new Rectangle((int)Projectile.position.X - 40, (int)Projectile.position.Y - 40, 80 + Projectile.width, 80 + Projectile.height));
                if (plr != -1 && plr != Projectile.owner)
                {
                    if (Main.player[plr].team == Main.player[Projectile.owner].team)
                    {
                        int healing = Main.DamageVar(Projectile.damage / 2f);
                        //Main.player[plr].HealEffect(healing, broadcast: false);
                        Main.player[plr].statLife += healing;
                        if (Main.player[plr].statLife > Main.player[plr].statLifeMax2)
                        {
                            Main.player[plr].statLife = Main.player[plr].statLifeMax2;
                        }
                        NetMessage.SendData(MessageID.PlayerHeal, -1, -1, null, plr, healing);
                        NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, plr, healing);
                        Projectile.Kill();
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Main.teamColor[Main.player[Projectile.owner].team].UseA(15);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
            var origin = new Vector2(0f, texture.Height / 2f);
            int trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
            var drawColor = Projectile.GetAlpha(lightColor);
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f / trailLength * i;
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset, null, drawColor * (1f - progress), Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, Projectile.position + offset, null, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}