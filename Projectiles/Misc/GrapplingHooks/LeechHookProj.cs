using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
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
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            connectedNPC = -1;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Utils.CenteredRectangle(hitbox.Center.ToVector2(), hitbox.Size() * 3.5f);
        }

        public override bool PreAI()
        {
            if (Projectile.originalDamage <= 0)
            {
                Projectile.originalDamage = Projectile.damage;
            }
            if (Projectile.damage < Projectile.originalDamage)
            {
                Projectile.damage = Projectile.originalDamage;
            }
            if (Projectile.position.HasNaNs())
            {
                Projectile.Kill();
            }
            if (connectedNPC > -1 && (!Main.npc[connectedNPC].active || ProjectileLoader.GrappleOutOfRange(Projectile.Distance(Main.player[Projectile.owner].Center) * 0.75f, Projectile)))
            {
                connectedNPC = -1;
            }
            Main.player[Projectile.owner].Aequus().leechHookNPC = connectedNPC;
            if (connectedNPC != -1)
            {
                Projectile.ai[0] = 2f;
                if (Main.myPlayer == Projectile.owner && Main.GameUpdateCount % 20 == 0)
                {
                    Main.player[Projectile.owner].Heal(2);
                }
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
            return true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.immortal ? false : null;
        }

        public override float GrappleRange()
        {
            return 272f;
        }

        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 14f;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = 10f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0.25f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[0] = 0f;
            connectedNPC = target.whoAmI;
            Projectile.tileCollide = false;
            Projectile.netUpdate = true;
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
            DrawChain(ModContent.Request<Texture2D>(Texture + "_Chain", AssetRequestMode.ImmediateLoad).Value, Projectile.Center, player.Center);
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
            float progress = 0f;
            for (int i = 0; i < 650; i++)
            {
                var diff = endPosition - currentPosition;
                float length = diff.Length();
                float scale = Projectile.scale;
                var color = Helper.GetColor(currentPosition);
                if (progress > 0.25f)
                {
                    scale *= Math.Max(1f - (progress - 0.25f) / 0.75f, 0.35f);
                }
                var addVelocity = velocity;
                if (progress <= 0.5f && progress > 0.2f)
                {
                    addVelocity = Vector2.Normalize(Vector2.Lerp(addVelocity, diff, (progress - 0.1f) / 0.2f) * 0.02f) * height;
                }
                else
                {
                    addVelocity = addVelocity.RotatedBy(Math.Sin(i * 0.1f + Main.GlobalTimeWrappedHourly * 6f + Projectile.Center.Length() / 64f) * 0.1f * scale);
                }
                currentPosition += addVelocity * scale;
                progress += 0.033f;
                if (length <= height)
                    break;
                Main.EntitySpriteDraw(chain, currentPosition - Main.screenPosition, null, color, addVelocity.ToRotation() + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(connectedNPC);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            connectedNPC = reader.ReadInt32();
        }
    }
}