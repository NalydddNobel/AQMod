using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.UserInterface;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using AQMod.Effects;
using AQMod.Effects.ScreenEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Dedicated.nalyddd
{
    public class Narrizuul : ModItem, IDedicatedItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 80;
            item.height = 80;
            item.damage = 777;
            item.knockBack = 7.77f;
            item.crit = 3;
            item.magic = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 7;
            item.useAnimation = 14;
            item.rare = ItemRarityID.Purple;
            item.shootSpeed = 27.77f;
            item.autoReuse = true;
            item.noMelee = true;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 50);
            item.mana = 7;
            item.shoot = ModContent.ProjectileType<NarrenBolt>();
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                UIManager.TextDrawingMethods.Narrizuul(line.text, line.X, line.Y, line.rotation, line.origin, line.baseScale);
                return false;
            }
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(MathHelper.PiOver4 * 0.5f), type, damage, knockBack, player.whoAmI); // kinda lazy so why not?
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(-MathHelper.PiOver4 * 0.5f), type, damage, knockBack, player.whoAmI);
            return true;
        }

        Color IDedicatedItem.DedicatedItemColor() => DedicatedColors.nalyddd;
    }

    public class NarrenBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 72;
            projectile.height = 72;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.timeLeft = 80;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = Main.rand.NextFloat(0.77f, 3.33f);
                projectile.localAI[1] = Main.rand.NextFloat(0, 120);
            }
            projectile.localAI[1] += Main.rand.NextFloat(0.01f, 0.1f);
            if (projectile.ai[1] < 6)
            {
                if (projectile.ai[0] == 0)
                    projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;
                projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(projectile.localAI[0]) * projectile.ai[0]);
                projectile.ai[1]++;
            }
            else
            {
                int targetIndex = AQNPC.FindClosest(projectile.Center, 1000f);
                if (targetIndex != -1)
                {
                    NPC target = Main.npc[targetIndex];
                    if (Vector2.Distance(projectile.Center, target.Center).Abs() > target.width)
                    {
                        projectile.timeLeft += 2;
                        projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(target.Center - projectile.Center) * 22.77f, 0.1f);
                    }
                }
                if (projectile.ai[1] < 15)
                {
                    if (projectile.ai[0] == 0)
                        projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;
                    projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(projectile.localAI[0]) * projectile.ai[0]);
                    projectile.ai[1]++;
                }
            }
            if (Main.rand.NextBool(4))
            {
                int d = Dust.NewDust(projectile.Center, 0, 0, ModContent.DustType<Content.Dusts.ColorlessTransparentOrb>());
                Main.dust[d].color = CommonUtils.MovingRainbow(projectile.localAI[1]) * 1.5f;
                Main.dust[d].velocity = Vector2.Zero;
            }
            Lighting.AddLight(projectile.Center, (CommonUtils.MovingRainbow(projectile.localAI[1]) * 1.5f).ToVector3() * projectile.scale);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = DrawUtils.LegacyTextureCache.Glows[GlowID.NarrenBolt];
            Vector2 origin = texture.Size() / 2f;
            Vector2 offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            float colorMult = 1f / ProjectileID.Sets.TrailCacheLength[projectile.type];
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            if (Trailshader.ShouldDrawVertexTrails(Trailshader.GetVertexDrawingContext_Projectile(projectile)))
            {
                var trueOldPos = new List<Vector2>();
                for (int i = 0; i < trailLength; i++)
                {
                    if (projectile.oldPos[i] == Vector2.Zero)
                    {
                        break;
                    }
                    trueOldPos.Add(ScreenShakeManager.UpsideDownScreenSupport(projectile.oldPos[i] + offset - Main.screenPosition));
                }
                if (trueOldPos.Count > 1)
                {
                    Trailshader trail = new Trailshader(TextureCache.Trails[TrailTextureID.ThickLine], Trailshader.TextureTrail);
                    var clr2 = CommonUtils.MovingRainbow(projectile.localAI[1]);
                    trail.PrepareVertices(trueOldPos.ToArray(), (p) => new Vector2(20f - p * 20f), (p) => clr2 * 0.65f * (1f - p));
                    trail.Draw();
                }
            }
            else
            {
                for (int i = 0; i < trailLength; i++)
                {
                    if (projectile.oldPos[i] == new Vector2(0f, 0f))
                        break;
                    float progress = 1f - 1f / trailLength * i;
                    var clr2 = CommonUtils.MovingRainbow(projectile.localAI[1] + i * 0.1f) * 0.225f;
                    var orig2 = Main.projectileTexture[projectile.type].Size() / 2f;
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset / 2f - Main.screenPosition, null, clr2 * progress, projectile.rotation, orig2, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            texture = TextureCache.Lights[LightID.Spotlight66x66];
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, CommonUtils.MovingRainbow(projectile.localAI[1]) * 0.5f, projectile.rotation, texture.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            texture = Main.projectileTexture[projectile.type];
            var drawPos = projectile.Center - Main.screenPosition;
            var orig = Main.projectileTexture[projectile.type].Size() / 2f;
            spriteBatch.Draw(texture, drawPos, null, new Color(250, 250, 250, 20), projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
            var clr = Color.Lerp(new Color(250, 250, 250, 20), CommonUtils.MovingRainbow(projectile.localAI[1]), 0.5f);
            spriteBatch.Draw(texture, drawPos, null, clr * 0.5f, projectile.rotation, orig, projectile.scale + 0.2f + (float)Math.Sin(Main.GlobalTime * 20f) * 0.4f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPos, null, clr * 0.32f, projectile.rotation + MathHelper.PiOver4, orig, projectile.scale + 0.1f + (float)Math.Sin(Main.GlobalTime * 20f + 1f) * 0.2f, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == projectile.owner && AQMod.TonsofScreenShakes)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center);
                if (distance < 800)
                {
                    ScreenShakeManager.AddEffect(new BasicScreenShake(8, AQMod.MultIntensity((int)(800f - distance) / 64)));
                }
            }
            Color color = CommonUtils.MovingRainbow(projectile.localAI[1]) * 1.5f;
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(projectile.Center, 0, 0, ModContent.DustType<ColorlessTransparentOrb>());
                Main.dust[d].scale *= Main.rand.NextFloat(1.1f, 1.7f);
                Main.dust[d].color = color;
                Main.dust[d].velocity = new Vector2(6, 0).RotatedByRandom(MathHelper.TwoPi);
            }
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(projectile.Center, 0, 0, ModContent.DustType<ColorlessTransparentOrb>());
                Main.dust[d].scale *= Main.rand.NextFloat(1.1f, 1.7f);
                Main.dust[d].color = CommonUtils.MovingRainbow(projectile.localAI[1] + i * 0.1f) * 1.5f;
                Main.dust[d].velocity = new Vector2(6, 0).RotatedByRandom(MathHelper.TwoPi);
            }
            if (Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) < Math.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight * Main.screenHeight))
                Main.PlaySound(SoundID.Item14, projectile.Center);
        }
    }
}