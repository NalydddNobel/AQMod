using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using AQMod.Effects;
using AQMod.Effects.ScreenEffects;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class MagmalbulbiaStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new DynamicGlowmaskOverlayData(CommonUtils.GetPath<MagmalbulbiaStaff>() + "_Glow", getGlowmaskColor), item.type);
        }

        private static Color getGlowmaskColor() => Color.Lerp(new Color(188, 175, 135, 0), new Color(144, 130, 111, 0), ((float)Math.Sin(Main.GlobalTime * 1.1f) + 1f) / 2f);

        public override void SetDefaults()
        {
            item.damage = 60;
            item.magic = true;
            item.useTime = 26;
            item.useAnimation = 26;
            item.width = 50;
            item.height = 50;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<DemonicFireball>();
            item.shootSpeed = 24.11f;
            item.mana = 11;
            item.autoReuse = true;
            item.UseSound = SoundID.Item95;
            item.value = AQItem.EnergyWeaponValue;
            item.knockBack = 6f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GebulbaStaff>());
            recipe.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 10);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class DemonicFireball : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.timeLeft = 200;
            projectile.magic = true;
            projectile.friendly = true;
            projectile.penetrate = 6;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.2f + projectile.velocity.Y.Abs() * 0.02f;
            if (projectile.velocity.Y > 30f)
                projectile.velocity.Y = 30f;
            if (projectile.ai[1] > 0)
            {
                projectile.ai[1]--;
                if (projectile.ai[1] == 0 && Main.npc[(int)projectile.ai[0]].active)
                {
                    Vector2 center = projectile.Center;
                    CollisionEffects(projectile.velocity);
                    projectile.position += projectile.velocity;
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy((Main.npc[(int)projectile.ai[0]].Center - center).ToRotation());
                }
            }
        }

        public override void PostAI()
        {
            Lighting.AddLight(projectile.Center, new Vector3(0.5f, 0.3f, 0.05f));
        }

        private void CollisionEffects(Vector2 velocity)
        {
            Vector2 spawnPos = projectile.position + velocity;
            if (Main.myPlayer == projectile.owner && AQMod.TonsofScreenShakes)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center);
                if (distance < 400)
                {
                    ScreenShakeManager.AddEffect(new BasicScreenShake(8, AQMod.MultIntensity((int)(400f - distance) / 32)));
                }
            }
            DemonicFireballExplosion.Explode(projectile.Center + projectile.velocity, projectile.damage / 2, projectile.damage, projectile.knockBack, projectile.owner);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.ai[0] = target.whoAmI;
            projectile.ai[1] = 15;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool doEffects = false;
            Vector2 center = projectile.Center;
            projectile.ai[0] = -1;
            projectile.ai[1] = 0;
            if (oldVelocity.Length() > 5f)
            {
                int npcChoice = -1;
                float shortestDistance = 800f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    float length = (Main.npc[i].Center - center).Length();
                    if (Main.npc[i].CanBeChasedBy() && length < shortestDistance && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                    {
                        npcChoice = i;
                        shortestDistance = length;
                    }
                }
                if (npcChoice > -1)
                {
                    projectile.ai[0] = npcChoice;
                    projectile.ai[1] = 15;
                    if (Main.npc[npcChoice].Center.Y < center.Y - 80)
                    {
                        CollisionEffects(projectile.velocity);
                        projectile.velocity = new Vector2(oldVelocity.Length() * 0.9f, 0f).RotatedBy((Main.npc[npcChoice].Center - center).ToRotation());
                        return false;
                    }
                }
            }
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = oldVelocity.X * -0.9f;
                doEffects = true;
            }
            if (projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 3f)
            {
                projectile.velocity.Y = oldVelocity.Y * -0.9f;
                doEffects = true;
            }
            if (doEffects)
            {
                CollisionEffects(projectile.velocity);
            }
            else
            {
                projectile.velocity *= 0.9f;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawpos = projectile.Center - Main.screenPosition;
            Texture2D texture = TextureCache.Lights[LightID.Spotlight66x66];
            Main.spriteBatch.Draw(texture, drawpos, null, new Color(180, 160, 20, 255), 0f, texture.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], drawpos, null, new Color(255, 255, 255, 200), 0f, Main.projectileTexture[projectile.type].Size() / 2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item85.WithPitchVariance(2f), projectile.position);
            for (int i = 0; i < 40; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
            }
            for (int i = 0; i < 6; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.FlameBurst);
                Main.dust[d].noGravity = false;
            }
        }
    }

    public class DemonicFireballExplosion : ModProjectile
    {
        public override string Texture => "AQMod/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.timeLeft = 2;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
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

        public static void Explode(Vector2 center, int size, int dmg, float kb, int owner)
        {
            int p = Projectile.NewProjectile(center, Vector2.Zero, ModContent.ProjectileType<DemonicFireballExplosion>(), dmg, kb, owner);
            if (p >= 0)
            {
                Main.projectile[p].width = size * 2;
                Main.projectile[p].height = size * 2;
                Vector2 position = center - new Vector2(size, size);
                Main.projectile[p].position = position;
                Main.PlaySound(SoundID.Item14, center);
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDust(position, Main.projectile[p].width, Main.projectile[p].height, 31);
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(position, Main.projectile[p].width, Main.projectile[p].height, DustID.Fire);
                }
                Dust.NewDust(position, Main.projectile[p].width, Main.projectile[p].height, DustID.FlameBurst);
            }
        }
    }
}