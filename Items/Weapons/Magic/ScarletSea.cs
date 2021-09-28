using AQMod.Assets.Enumerators;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class ScarletSea : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 15;
            item.magic = true;
            item.knockBack = 2.45f;
            item.width = 40;
            item.height = 40;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 6;
            item.useAnimation = 12;
            item.reuseDelay = 9;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BloodOrb>();
            item.shootSpeed = 8.45f;
            item.noMelee = true;
            item.UseSound = SoundID.Item8;
            item.mana = 5;
            item.value = Item.sellPrice(gold: 1);
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var staffDirection = new Vector2(speedX, speedY);
            var center = Vector2.Normalize(staffDirection) * (45f * item.scale);
            if (Main.myPlayer == player.whoAmI)
            {
                int count = 8;
                float rot = MathHelper.PiOver4 / count;
                const float piOver8 = MathHelper.PiOver4 / 2f;
                float rot2 = staffDirection.ToRotation();
                var dustType = ModContent.DustType<MonoDust>();
                var color = new Color(205, 15, 15, 0);
                float off = 5;
                float dustSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                var plrCenter = player.Center;
                for (int i = 0; i < count; i++)
                {
                    var normal = new Vector2(1f, 0f).RotatedBy(rot * i - piOver8 + rot2);
                    int d = Dust.NewDust(plrCenter + center + normal * off, 2, 2, dustType, 0f, 0f, 0, color);
                    Main.dust[d].velocity = normal * dustSpeed;
                }
            }
            if (player.ownedProjectileCounts[item.shoot] > 10)
            {
                int count = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == item.shoot && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].ai[0] >= 0f)
                    {
                        count++;
                        if (count >= 10)
                        {
                            Main.projectile[i].ai[0] = -1f;
                            ((BloodOrb)Main.projectile[i].modProjectile).StopFollowingMouse(Main.projectile[i].Center + Vector2.Normalize(Main.projectile[i].velocity) * 195f);
                        }
                    }
                }
            }
            var newVelocity = new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
            position += Vector2.Normalize(staffDirection) * (50f * item.scale);
            speedX = newVelocity.X;
            speedY = newVelocity.Y;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimtaneBar, 8);
            recipe.AddIngredient(ItemID.Ruby, 2);
            recipe.AddIngredient(ItemID.Sapphire, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class BloodOrb : ModProjectile
    {
        private const int StopFollowingMouseDistance = 200;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 18;
        }

        private Vector2 _oldMousePos;

        public void StopFollowingMouse(Vector2 mousePosition)
        {
            _oldMousePos = mousePosition;
            projectile.ai[0] = -1f;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.magic = true;
            projectile.penetrate = 3;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 1500;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 0f)
            {
                return;
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[0] = projectile.velocity.Length();
            }
            if (Main.myPlayer == projectile.owner)
            {
                projectile.velocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, Main.MouseWorld - projectile.Center, 0.02f)) * projectile.ai[0];
                if (Vector2.Distance(projectile.Center, Main.MouseWorld) < StopFollowingMouseDistance)
                {
                    StopFollowingMouse(Main.MouseWorld);
                }
                projectile.netUpdate = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item118, projectile.position);
            int count = 20;
            float rot = MathHelper.TwoPi / count;
            var center = projectile.Center + new Vector2(-1f, -1f);
            var type = ModContent.DustType<MonoDust>();
            var color = new Color(205, 15, 15, 0);
            float off = projectile.width / 2f;
            float dustSpeed = projectile.velocity.Length() / 2f;
            for (int i = 0; i < count; i++)
            {
                var normal = new Vector2(1f, 0f).RotatedBy(rot * i);
                int d = Dust.NewDust(center + normal * off, 2, 2, type, 0f, 0f, 0, color);
                Main.dust[d].velocity = normal * dustSpeed;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var offset = new Vector2(projectile.width / 2f - Main.screenPosition.X, projectile.height / 2f - Main.screenPosition.Y);
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var origin = frame.Size() / 2f;
            var color = new Color(250, 250, 250, 128);
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            color *= 0.5f;
            float colorMult = 1f / trailLength;
            for (int i = 0; i < trailLength; i++)
            {
                if (projectile.oldPos[i] == Vector2.Zero)
                {
                    break;
                }
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset, frame, color * (colorMult * (trailLength - i + 1)), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            if (projectile.ai[0] < 0f && _oldMousePos.X != -1f)
            {
                float dist = Vector2.Distance(projectile.Center, _oldMousePos);
                if (dist < 200f)
                {
                    colorMult = 1f - dist / StopFollowingMouseDistance;
                    texture = DrawUtils.Textures.Lights[LightID.Spotlight10x50];
                    frame = new Rectangle(0, 0, texture.Width, texture.Height);
                    color = new Color(205, 15, 15, 0) * colorMult;
                    origin = texture.Size() / 2f;

                    var texture2 = DrawUtils.Textures.Lights[LightID.Spotlight20x20];

                    Main.spriteBatch.Draw(texture2, projectile.position + offset, null, color, projectile.rotation, texture2.Size() / 2f, projectile.scale * (colorMult * colorMult), SpriteEffects.None, 0f);

                    var scale = new Vector2(projectile.scale * (0.55f - (1f - colorMult) * 0.2f), projectile.scale * (0.95f - (1f - colorMult) * 0.55f));

                    Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0f);

                    scale *= 0.9f;

                    Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 1.4f, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 1.4f, projectile.rotation + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0f);

                    scale *= 0.9f;
                    scale *= 1f - (float)Math.Sin(projectile.timeLeft) * 0.1f;

                    Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.8f, projectile.rotation + MathHelper.PiOver4, origin, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.8f, projectile.rotation + MathHelper.PiOver4 * 3f, origin, scale, SpriteEffects.None, 0f);
                }
                else
                {
                    _oldMousePos.X = -1f;
                }
            }
            return false;
        }
    }
}