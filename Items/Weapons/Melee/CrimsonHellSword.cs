using AQMod.Assets;
using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Effects;
using AQMod.Effects.SpriteBatchModifers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class CrimsonHellSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new Glowmask(GlowID.Burnterizer, new Color(200, 200, 200, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 35;
            item.useTime = 72;
            item.useAnimation = 24;
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 1);
            item.melee = true;
            item.knockBack = 3f;
            item.shoot = ModContent.ProjectileType<BurnterizerProjectile>();
            item.shootSpeed = 8f;
            item.scale = 1.35f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(lightColor.R * lightColor.R, lightColor.G * lightColor.G, lightColor.B * lightColor.B, lightColor.A);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.ownedProjectileCounts[item.shoot] <= 0)
            {
                int size = 16;
                var off = new Vector2(-size / 2f);
                var velo = new Vector2(speedX, speedY) * 0.1f;
                for (int i = 0; i < 25; i++)
                {
                    int dustType = Main.rand.NextBool(5) ? 31 : DustID.Fire;
                    var p = position + new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) + off;
                    int d = Dust.NewDust(p, size, size, dustType);
                    Main.dust[d].velocity = Vector2.Lerp(Main.dust[d].velocity, velo, 0.3f);
                    if (dustType != 31)
                        Main.dust[d].scale = Main.rand.NextFloat(0.8f, 2f);
                    Main.dust[d].noGravity = true;
                }
                Main.PlaySound(SoundID.DD2_BetsyFireballShot, position);
                return true;
            }
            return false;
        }

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<AQPlayer>().burnterizerCursor = true;
        }
    }

    public class BurnterizerSights : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.noEnchantments = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
        }

        public override void AI()
        {
            var player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (aQPlayer.burnterizerCursor)
            {
                projectile.timeLeft = 2;
                if (player.ownedProjectileCounts[player.HeldItem.shoot] > 0)
                {
                    if ((int)projectile.ai[0] == 0)
                    {
                        int shootProj = player.HeldItem.shoot;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            var p = Main.projectile[i];
                            if (p.active && p.type == shootProj && p.owner == player.whoAmI)
                            {
                                projectile.ai[0] = i + 1;
                            }
                        }
                    }
                    if ((int)projectile.ai[1] == 0)
                    {
                        int target = AQNPC.FindClosest(projectile.Center, 400f);
                        if (target != -1)
                        {
                            float dist = Vector2.Distance(Main.npc[target].Center, projectile.Center);
                            if (dist <= 2f)
                            {
                                projectile.velocity = Vector2.Zero;
                                projectile.Center = Main.npc[target].Center;
                            }
                            else
                            {
                                projectile.velocity = Vector2.Normalize(Main.npc[target].Center - projectile.Center) * (400f - dist) / 80f;
                            }
                        }
                        int proj = (int)projectile.ai[0] - 1;
                        float dist2 = Vector2.Distance(Main.projectile[proj].Center, projectile.Center);
                        if (dist2 >= Main.projectile[proj].width)
                        {
                            float projSpeed = Main.projectile[proj].velocity.Length() + 0.2f;
                            float maxSpeed = 16f + dist2 / (Main.projectile[proj].width / 8f);
                            if (projSpeed > maxSpeed)
                            {
                                projSpeed = maxSpeed;
                            }
                            Main.projectile[proj].velocity = Vector2.Normalize(Vector2.Lerp(Main.projectile[proj].velocity, projectile.Center - Main.projectile[proj].Center, 0.02f)) * projSpeed;
                        }
                        else
                        {
                            projectile.velocity = new Vector2(0f, 0f);
                            projectile.ai[1] = 1f;
                        }
                    }
                }
                else
                {
                    if (Main.myPlayer == projectile.owner)
                    {
                        projectile.Center = Main.MouseWorld;
                    }
                    projectile.velocity = new Vector2(0f, 0f);
                    projectile.ai[0] = 0f;
                    projectile.ai[1] = 0f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if ((int)projectile.ai[0] == 0)
            {
                if (Main.myPlayer != projectile.owner)
                {
                    return false;
                }
            }
            if (AQConfigClient.Instance.SpotlightShader)
            {
                var batcher = new GeneralEntityBatcher(Main.spriteBatch);
                batcher.StartShaderBatch();
                float intensity = 1f;
                if ((int)projectile.ai[1] == 0)
                {
                    if ((int)projectile.ai[0] != 0)
                    {
                        int target = AQNPC.FindClosest(projectile.Center, 400f);
                        if (target != -1)
                        {
                            var diff = Main.npc[target].Center - projectile.Center;
                            float dist = diff.Length();
                            if (dist < 400f)
                            {
                                float intensity2 = 1f - dist / 400f;
                                var normal = Vector2.Normalize(diff);
                                var pos = projectile.Center - Main.screenPosition + normal * dist / 2f;
                                SDraw.Light(pos, diff.ToRotation() + MathHelper.PiOver2, new Vector2(40f, dist), new Color(255, 150, 10, 100) * intensity2);
                            }
                        }
                        int proj = (int)projectile.ai[0] - 1;
                        float distance = Vector2.Distance(Main.projectile[proj].Center, projectile.Center);
                        if (distance < Main.projectile[proj].width)
                        {
                            intensity *= distance / Main.projectile[proj].width;
                        }
                    }
                    else
                    {
                        var center = projectile.Center;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].CanBeChasedBy())
                            {
                                var diff = Main.npc[i].Center - center;
                                float dist = diff.Length();
                                if (dist < 400f)
                                {
                                    float intensity2 = 1f - dist / 400f;
                                    var normal = Vector2.Normalize(diff);
                                    var pos = projectile.Center - Main.screenPosition + normal * dist / 2f;
                                    SDraw.Light(pos, diff.ToRotation() + MathHelper.PiOver2, new Vector2(40f, dist), new Color(255, 150, 10, 100) * intensity2);
                                }
                            }
                        }
                    }
                }
                else
                {
                    intensity = 0.4f;
                }
                if (intensity < 0.4f)
                {
                    intensity = 0.4f;
                }
                SDraw.Light(projectile.Center - Main.screenPosition, 40f, new Color(180, 25, 25, 10) * intensity);
                SDraw.Light(projectile.Center - Main.screenPosition, 60f, new Color(125, 10, 10, 0) * intensity);
                batcher.StartBatch();
            }
            else
            {
                return true;
            }
            return false;
        }
    }

    public class BurnterizerProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.melee = true;
            projectile.noEnchantments = true;
            projectile.friendly = true;
            projectile.timeLeft = 120;
            projectile.penetrate = 5;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 5;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.damage = (int)(projectile.damage * 0.85f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 80);
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
            Main.dust[d].velocity = -projectile.velocity * 0.1f;
            Main.dust[d].scale = Main.rand.NextFloat(0.8f, 2f);
            Main.dust[d].noGravity = true;
            if (Main.rand.NextBool(5))
            {
                d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31);
                Main.dust[d].velocity = -projectile.velocity * 0.1f;
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 2f);
                Main.dust[d].noGravity = true;
            }
            Lighting.AddLight(projectile.Center, new Vector3(1f, 0.5f, 0.1f));
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            int p = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BurnterizerExplosion>(), 50, projectile.knockBack, projectile.owner);
            Vector2 position = projectile.Center - new Vector2(Main.projectile[p].width / 2f, Main.projectile[p].height / 2f);
            Main.projectile[p].position = position;
            var bvelo = -projectile.velocity * 0.375f;
            for (int i = 0; i < 6; i++)
            {
                Gore.NewGore(Main.projectile[p].Center, bvelo * 0.2f, 61 + Main.rand.Next(3));
            }
            for (int i = 0; i < 24; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, 31);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 60; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, DustID.Fire);
                Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var drawPosition = projectile.Center - Main.screenPosition;
            var color = projectile.GetAlpha(lightColor);
            var origin = new Vector2(texture.Width / 2f, 10f);
            var drawData = new DrawData(texture, drawPosition, null, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0);
            var batcher = new GeneralEntityBatcher(Main.spriteBatch);
            bool resetBatch = false;
            if (AQConfigClient.Instance.OutlineShader)
            {
                resetBatch = true;
                batcher.StartShaderBatch();
                float intensity = (float)Math.Sin(Main.GlobalTime * 10f) + 1.5f;
                var effect = GameShaders.Misc["AQMod:OutlineColor"];
                effect.UseColor(new Vector3(1f, 0.5f * intensity, 0.1f * intensity));
                effect.Apply(drawData);
            }
            drawData.Draw(Main.spriteBatch);
            if (resetBatch)
                batcher.StartBatch();
            drawData.scale *= 1.25f;
            drawData.color *= 0.25f;
            drawData.Draw(Main.spriteBatch);
            return false;
        }
    }

    public class BurnterizerExplosion : ModProjectile
    {
        public override string Texture => "AQMod/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.timeLeft = 2;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[0] > 0)
            {
                projectile.active = false;
            }
            projectile.ai[0]++;
        }
    }
}