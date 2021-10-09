using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common.ItemOverlays;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class HorizonSkies : ModItem
    {
        public static Color BlueMonoDustColor => new Color(144, 144, 255, 128);
        public static Color OrangeMonoDustColor => new Color(150, 110, 66, 128);

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyGlowmask(GlowID.HorizonSkies), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.melee = true;
            item.knockBack = 5.45f;
            item.rare = ItemRarityID.LightPurple;
            item.damage = 62;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<HorizonSkiesProjectile>();
            item.shootSpeed = 7f;
            item.value = Item.sellPrice(gold: 5);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] < 1;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HallowedBar, 15);
            recipe.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            recipe.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 10);
            recipe.AddIngredient(ItemID.SoulofLight, 8);
            recipe.AddIngredient(ItemID.SoulofNight, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class HorizonSkiesProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.aiStyle = 19;
            projectile.penetrate = -1;
            projectile.alpha = 0;
            projectile.hide = true;
            projectile.ownerHitCheck = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player projOwner = Main.player[projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            projectile.direction = projOwner.direction;
            projOwner.heldProj = projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            projectile.position.X = ownerMountedCenter.X - projectile.width / 2;
            projectile.position.Y = ownerMountedCenter.Y - projectile.height / 2;
            if (!projOwner.frozen)
            {
                if (projectile.ai[0] == 0f)
                {
                    projectile.ai[0] = 2f;
                    projectile.velocity = projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f));
                    projectile.netUpdate = true;
                }
                if (projOwner.itemAnimation < projOwner.itemAnimationMax / 3)
                {
                    projectile.ai[0] -= 2.4f;
                }
                else
                {
                    projectile.ai[0] += 0.9f;
                }
            }
            projectile.position += projectile.velocity * projectile.ai[0];
            if (projOwner.itemAnimation < projOwner.itemAnimationMax / 5 * 3)
            {
                if (projectile.ai[1] == 0f)
                {
                    Vector2 center = projectile.Center;
                    float rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    Projectile.NewProjectile(center, projectile.velocity * 1.2f, ModContent.ProjectileType<HorizonSkiesBolt>(), projectile.damage, projectile.knockBack, projectile.owner, MathHelper.PiOver2);
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(center + new Vector2((float)Math.Sin(i) * 10f, 0f).RotatedBy(rotation), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, HorizonSkies.BlueMonoDustColor);
                        Main.dust[d].velocity += projectile.velocity;
                        Main.dust[d].noGravity = true;
                        d = Dust.NewDust(center + new Vector2((float)Math.Cos(i) * 10f, 0f).RotatedBy(rotation), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, HorizonSkies.OrangeMonoDustColor);
                        Main.dust[d].velocity += projectile.velocity;
                        Main.dust[d].noGravity = true;
                    }
                    Main.PlaySound(SoundID.Trackable, (int)ownerMountedCenter.X, (int)ownerMountedCenter.Y, 28 + Main.rand.Next(3), 0.5f, -1f);
                }
                projectile.ai[1] = 1f;
            }
            if (projOwner.itemAnimation == 0)
                projectile.Kill();
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4 * 3f;
            if (projectile.spriteDirection == -1)
                projectile.rotation -= MathHelper.PiOver2;
        }
    }

    public class HorizonSkiesBolt : ModProjectile
    {
        public override string Texture => "AQMod/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.extraUpdates = 5;
        }

        private void Hit(Vector2 position)
        {
            if (projectile.ai[1] > 0f)
                return;
            int p = Projectile.NewProjectile(position, new Vector2(0f, 0f), ModContent.ProjectileType<HorizonSkiesExplosion>(), projectile.damage, projectile.knockBack, projectile.owner);
            Vector2 realPos = new Vector2(position.X - Main.projectile[p].width / 2f, position.Y - Main.projectile[p].height / 2f);
            Main.projectile[p].position = realPos;
            Main.projectile[p].direction = projectile.velocity.X < 0f ? -1 : 1;
            for (int i = 0; i < 50; i++)
            {
                int d = Dust.NewDust(realPos, Main.projectile[p].width, Main.projectile[p].height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, HorizonSkies.BlueMonoDustColor);
                Main.dust[d].velocity = Vector2.Normalize(Main.dust[d].position - position) * 10f;
                Main.dust[d].noGravity = true;
                d = Dust.NewDust(realPos, Main.projectile[p].width, Main.projectile[p].height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, HorizonSkies.OrangeMonoDustColor);
                Main.dust[d].velocity = Vector2.Normalize(Main.dust[d].position - position) * 10f;
                Main.dust[d].noGravity = true;
            }
            Main.PlaySound(SoundID.Item14, realPos);
            projectile.ai[1] = 10f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = projectile.width / 4;
            height = projectile.height / 4;
            fallThrough = true;
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Hit(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Hit(target.Center);
        }

        public override void Kill(int timeLeft)
        {
            Hit(projectile.Center);
        }

        public override void AI()
        {
            projectile.ai[1]--;
            projectile.velocity.Y += 0.04f;
            projectile.velocity.X *= 0.9999f;
            Vector2 center = projectile.Center;
            int d = Dust.NewDust(center + new Vector2((float)Math.Sin(projectile.ai[0]) * 25f, 0f).RotatedBy(projectile.rotation + MathHelper.PiOver2), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, HorizonSkies.BlueMonoDustColor);
            Main.dust[d].velocity *= 0.01f;
            Main.dust[d].noGravity = true;
            d = Dust.NewDust(center + new Vector2((float)Math.Sin(projectile.ai[0] + MathHelper.Pi) * 25f, 0f).RotatedBy(projectile.rotation + MathHelper.PiOver2), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, HorizonSkies.OrangeMonoDustColor);
            Main.dust[d].velocity *= 0.01f;
            Main.dust[d].noGravity = true;
            projectile.ai[0] += 0.314f;
            projectile.rotation = projectile.velocity.ToRotation();
        }
    }

    public class HorizonSkiesExplosion : ModProjectile
    {
        public override string Texture => "AQMod/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.timeLeft = 5;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.manualDirectionChange = true;
        }

        public override void AI()
        {
            var center = projectile.Center;
            int d = Dust.NewDust(center + new Vector2((float)Math.Sin(projectile.ai[0]) * 25f, 0f).RotatedBy(projectile.rotation + MathHelper.PiOver2), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, HorizonSkies.BlueMonoDustColor);
            Main.dust[d].noGravity = true;
            d = Dust.NewDust(center + new Vector2((float)Math.Cos(projectile.ai[0]) * 25f, 0f).RotatedBy(projectile.rotation + MathHelper.PiOver2), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, HorizonSkies.OrangeMonoDustColor);
            Main.dust[d].noGravity = true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            hitDirection = projectile.direction;
        }
    }
}