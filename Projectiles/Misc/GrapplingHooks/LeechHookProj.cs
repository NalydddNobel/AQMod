using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.GrapplingHooks
{
    public class LeechHookProj : MeathookProj
    {
        public LeechHookProj() : base()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 7;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 10;
            connectedNPC = -1;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Utils.CenteredRectangle(hitbox.Center.ToVector2(), hitbox.Size() * 3.5f);
        }

        public override bool PreAI()
        {
            if (connectedNPC > -1 && (!Main.npc[connectedNPC].active || ProjectileLoader.GrappleOutOfRange(Projectile.Distance(Main.player[Projectile.owner].Center) * 0.75f, Projectile)))
            {
                connectedNPC = -1;
            }
            Main.player[Projectile.owner].Aequus().leechHookNPC = connectedNPC;
            if (connectedNPC != -1)
            {
                Projectile.ai[0] = 2f;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].aiStyle == ProjAIStyleID.Hook && Main.projectile[i].owner == Projectile.owner)
                    {
                        Main.projectile[i].Kill();
                    }
                }
                if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.Kill();
                    return false;
                }
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Main.npc[connectedNPC].Center;
                Projectile.rotation = (Projectile.Center - Main.player[Projectile.owner].Center).ToRotation() + MathHelper.PiOver2;
                return false;
            }
            else
            {
                int target = Projectile.FindTargetWithLineOfSight(150f);
                if (target != -1)
                {
                    if (Main.player[Projectile.owner].Distance(Main.npc[target].Center) > Main.player[Projectile.owner].Distance(Projectile.Center))
                    {
                        Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Main.player[Projectile.owner].DirectionTo(Main.npc[target].Center) * Projectile.velocity.Length(), 0.4f)) * Projectile.velocity.Length();
                    }
                }
            }
            return true;
        }

        public override float GrappleRange()
        {
            return 320f;
        }

        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 14f;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = 10f;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            knockback = Math.Min(knockback, 0.25f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.ai[0] = 0f;
            connectedNPC = target.whoAmI;
            Projectile.tileCollide = false;
        }

        public override bool PreDrawExtras()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var player = Main.player[Projectile.owner];
            float playerLength = (player.Center - Projectile.Center).Length();
            var texture = TextureAssets.Projectile[Type].Value;
            var drawPosition = Projectile.Center - Main.screenPosition;
            DrawChain(ModContent.Request<Texture2D>(Texture + "_Chain").Value, Projectile.Center, player.Center);
            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            return false;
        }

        public void DrawChain(Texture2D chain, Vector2 currentPosition, Vector2 endPosition)
        {
            float range = GrappleRange();
            int height = chain.Height - 2;
            var velocity = endPosition - currentPosition;
            velocity.Normalize();
            velocity *= height;
            float rotation = velocity.ToRotation() + MathHelper.PiOver2;
            var origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
            for (int i = 0; i < 650; i++)
            {
                var diff = endPosition - currentPosition;
                float length = diff.Length();
                float progress = MathHelper.Clamp(1f - length / range, 0.1f, 1f);
                float scale = Projectile.scale;
                var color = AequusHelpers.GetColor(currentPosition);
                if (progress > 0.25f)
                {
                    scale *= Math.Max(1f - (progress - 0.25f) / 0.75f, 0.35f);
                }
                var addVelocity = velocity;
                if (progress <= 0.5f)
                {
                    addVelocity = Vector2.Normalize(Vector2.Lerp(addVelocity, diff, (progress - 0.1f) / 0.2f) * 0.02f) * height;
                }
                else
                {
                    addVelocity = addVelocity.RotatedBy(Math.Sin(i * 0.1f + Main.GlobalTimeWrappedHourly * 6f + Projectile.Center.Length() / 64f) * 0.1f * scale);
                }
                currentPosition += addVelocity * scale;
                if (length <= height)
                    break;
                Main.EntitySpriteDraw(chain, currentPosition - Main.screenPosition, null, color, addVelocity.ToRotation() + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0);
            }
        }
    }
}