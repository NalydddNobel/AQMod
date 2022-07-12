using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Bobbers
{
    public class BeeBobber : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BobberWooden);
            DrawOriginOffsetY = -8;
        }

        public Vector2 gotoPosition = new Vector2(0f, 0f);
        public float visualRotation = 0f;
        public byte dropDelay = byte.MaxValue;

        public override bool PreAI()
        {
            if (gotoPosition.X <= -1f && gotoPosition.Y <= -1f)
            {
                if (dropDelay != byte.MaxValue)
                {
                    Projectile.timeLeft = 10;
                    dropDelay--;
                    if (dropDelay <= 0f)
                        dropDelay = byte.MaxValue;
                    return false;
                }
                if (Projectile.honeyWet && Projectile.ai[1] < 0f)
                {
                    Projectile.ai[1] += 5;
                    if (Projectile.ai[1] >= 0f)
                    {
                        Projectile.ai[1] = 0f;
                        Projectile.localAI[1] = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                return true;
            }
            Projectile.timeLeft = 10;
            if (Main.myPlayer == Projectile.owner && gotoPosition == new Vector2(0f, 0f))
                gotoPosition = Main.MouseWorld + new Vector2(0f, -20f);
            if ((Projectile.Center - gotoPosition).Length() < 10f)
            {
                gotoPosition = new Vector2(-1f, -1f);
                Projectile.velocity *= 0.1f;
                visualRotation = Projectile.rotation;
                dropDelay = 20;
                Projectile.netUpdate = true;
            }
            else
            {
                var rect = Projectile.getRect();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && i != Projectile.whoAmI && Projectile.type == Main.projectile[i].type && Projectile.owner == Main.projectile[i].owner
                        && Projectile.Colliding(rect, Main.projectile[i].getRect()))
                    {
                        var velocityAddition = Main.projectile[i].DirectionTo(Projectile.Center).UnNaN() * 0.1f;
                        gotoPosition += velocityAddition * 50f;
                        gotoPosition.X -= Projectile.direction;
                        gotoPosition.Y -= 2f;
                        Projectile.velocity += velocityAddition;
                    }
                }
                Projectile.velocity = Vector2.Normalize(gotoPosition - Projectile.Center) * Main.player[Projectile.owner].HeldItem.shootSpeed;
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            }
            return false;
        }

        public override void PostAI()
        {
            if (gotoPosition.X == -1f && gotoPosition.Y == -1f)
            {
                visualRotation = visualRotation.AngleLerp(0f, 0.1f);
                Projectile.rotation = visualRotation;
            }
            if (Projectile.wet)
                gotoPosition = new Vector2(-2f, -2f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            gotoPosition = new Vector2(-2f, -2f);
            return true;
        }

        public override bool PreDrawExtras()
        {
            var player = Main.player[Projectile.owner];
            if (!Projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
                return false;
            AequusHelpers.DrawFishingLine(player, Projectile.position, Projectile.width / 2, Projectile.height, Projectile.velocity, Projectile.localAI[0], Main.player[Projectile.owner].Center + new Vector2(54f * Main.player[Projectile.owner].direction, -40f),
                Projectile.whoAmI % 2 == 0 ? new Color(255, 255, 0, 255) : new Color(20, 0, 20, 255));
            return false;
        }
    }
}