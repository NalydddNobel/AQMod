using AQMod.Assets;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.GrapplingHooks
{
    public sealed class StriderHook : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 0;
            item.knockBack = 7f;
            item.shoot = ModContent.ProjectileType<StriderHookHook>();
            item.shootSpeed = 8f;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.useAnimation = 20;
            item.useTime = 20;
            item.rare = ItemRarityID.Blue;
            item.noMelee = true;
            item.value = Item.sellPrice(silver: 20);
        }
    }

    public sealed class StriderHookHook : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 7;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 10;
        }

        public override void PostAI()
        {
            if (projectile.velocity.Length() > 0.01f)
            {
                projectile.velocity.Y += 0.08f;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }

        public override bool? CanUseGrapple(Player player)
        {
            int hooksOut = 0;
            for (int l = 0; l < Main.maxProjectiles; l++)
            {
                if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == projectile.type)
                    hooksOut++;
            }
            if (hooksOut > grappleHookCount(player))
                return false;
            return true;
        }

        public override void UseGrapple(Player player, ref int type)
        {
            int hooksOut = 0;
            int oldestHookIndex = -1;
            int oldestHookTimeLeft = 100000;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == projectile.whoAmI && Main.projectile[i].type == projectile.type)
                {
                    hooksOut++;
                    if (Main.projectile[i].timeLeft < oldestHookTimeLeft)
                    {
                        oldestHookIndex = i;
                        oldestHookTimeLeft = Main.projectile[i].timeLeft;
                    }
                }
            }
            if (hooksOut > grappleHookCount(player))
            {
                Main.projectile[oldestHookIndex].Kill();
            }
        }

        public override float GrappleRange()
        {
            return 550f;
        }

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = grappleHookCount(player);
        }

        public int grappleHookCount(Player player)
        {
            if (player.GetModPlayer<AQPlayer>().striderPalms2)
            {
                return 3;
            }
            return 2;
        }

        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 15f;
            if (player.GetModPlayer<AQPlayer>().striderPalms2)
            {
                speed *= 1.25f;
            }
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = 10f;
            if (player.GetModPlayer<AQPlayer>().striderPalms2)
            {
                speed *= 1.25f;
            }
        }

        public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY)
        {
            Vector2 dirToPlayer = projectile.DirectionTo(player.Center);
            if (dirToPlayer.HasNaNs())
            {
                grappleX += 100f;
                grappleY += 100f;
                return;
            }
            grappleX += dirToPlayer.X * 60f;
            grappleY += dirToPlayer.Y * 60f * player.ownedProjectileCounts[projectile.type];
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var player = Main.player[projectile.owner];
            float playerLength = (player.Center - projectile.Center).Length();
            var chainTexture = TextureCache.StriderHookHookChain.GetValue();
            float textureHeight = chainTexture.Height - 2f;
            DrawMethods.DrawChain_UseLighting(chainTexture, projectile.Center, player.Center, Main.screenPosition);
            var texture = TextureCache.GetTexture(projectile);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}