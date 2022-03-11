using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged.Healing
{
    public sealed class HealingBolt : ModProjectile
    {
        private Color _teamColor = Color.White;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.extraUpdates = 8;
            _teamColor = Color.White;
        }

        public override void AI()
        {
            _teamColor = Main.teamColor[Main.player[projectile.owner].team];
            projectile.velocity.Y += 0.0005f;
            projectile.rotation = projectile.velocity.ToRotation();
            if (Main.netMode != NetmodeID.SinglePlayer && Main.myPlayer == projectile.owner)
            {
                int plr = AQUtils.CheckForPlayers(new Rectangle((int)projectile.position.X - 40, (int)projectile.position.Y - 40, 80 + projectile.width, 80 + projectile.height));
                if (plr != -1 && plr != projectile.owner)
                {
                    if (Main.player[plr].team == Main.player[projectile.owner].team)
                    {
                        int healing = Main.DamageVar(projectile.damage / 2f);
                        //Main.player[plr].HealEffect(healing, broadcast: false);
                        Main.player[plr].statLife += healing;
                        if (Main.player[plr].statLife > Main.player[plr].statLifeMax2)
                        {
                            Main.player[plr].statLife = Main.player[plr].statLifeMax2;
                        }
                        NetMessage.SendData(MessageID.HealEffect, -1, -1, null, plr, healing);
                        NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, plr, healing);
                        projectile.Kill();
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return _teamColor.UseA(15);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f) - Main.screenPosition;
            var origin = new Vector2(0f, texture.Height / 2f);
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            var drawColor = projectile.GetAlpha(lightColor);
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f / trailLength * i;
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset, null, drawColor * (1f - progress), projectile.oldRot[i], origin, projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, projectile.position + offset, null, drawColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}