using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.GrapplingHooks
{
    public class MeathookProj : ModProjectile
    {
        public int connectedNPC;

        public MeathookProj()
        {
            connectedNPC = -1;
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
            Projectile.extraUpdates = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            connectedNPC = -1;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Utils.CenteredRectangle(hitbox.Center.ToVector2(), hitbox.Size() * 6f);
        }

        public override bool PreAI()
        {
            if (Projectile.position.HasNaNs())
            {
                Projectile.Kill();
            }
            if (connectedNPC > -1 && !Main.npc[connectedNPC].active)
            {
                connectedNPC = -1;
            }
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
                Projectile.Center = Main.npc[connectedNPC].Center;
                float distance = Projectile.Distance(Main.player[Projectile.owner].Center);
                if (Main.player[Projectile.owner].grapCount < 10)
                {
                    if (distance < Main.npc[connectedNPC].Size.Length() * 2f)
                    {
                        Main.player[Projectile.owner].immune = true;
                        Main.player[Projectile.owner].immuneTime = 12;
                    }
                    if (distance < 64f)
                    {
                        Projectile.Kill();
                        return false;
                    }
                    Main.player[Projectile.owner].grappling[Main.player[Projectile.owner].grapCount] = Projectile.whoAmI;
                    Main.player[Projectile.owner].grapCount++;
                }
                return false;
            }
            if ((int)Projectile.ai[0] == 1)
            {
                Projectile.damage = 0;
            }
            return true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.immortal ? false : null;
        }

        public override bool? SingleGrappleHook(Player player)
        {
            return true;
        }

        public override bool? CanUseGrapple(Player player)
        {
            for (int l = 0; l < Main.maxProjectiles; l++)
            {
                if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type)
                {
                    return (int)Main.projectile[l].ai[0] == 2;
                }
            }
            return true;
        }

        public override void UseGrapple(Player player, ref int type)
        {
            int hooksOut = 0;
            int oldestHookIndex = -1;
            int oldestHookTimeLeft = 100000;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.whoAmI && Main.projectile[i].type == Projectile.type)
                {
                    hooksOut++;
                    if (Main.projectile[i].timeLeft < oldestHookTimeLeft)
                    {
                        oldestHookIndex = i;
                        oldestHookTimeLeft = Main.projectile[i].timeLeft;
                    }
                }
            }
            if (hooksOut > 1)
                Main.projectile[oldestHookIndex].Kill();
        }

        public override float GrappleRange()
        {
            return 480f;
        }

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 1;
        }

        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 12f;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = connectedNPC > 0 ? 13f : 10f;
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
            AequusHelpers.DrawChain(ModContent.Request<Texture2D>(Texture + "_Chain", AssetRequestMode.ImmediateLoad).Value, Projectile.Center, player.Center, Main.screenPosition);
            var texture = TextureAssets.Projectile[Type].Value;
            var drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            return false;
        }
    }
}