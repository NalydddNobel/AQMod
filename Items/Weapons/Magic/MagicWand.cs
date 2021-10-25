using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.ItemOverlays;
using AQMod.Common.WorldGeneration;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class MagicWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyLegacyGlowmask(GlowID.MagicWand, new Color(128, 128, 128, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.magic = true;
            item.damage = 68;
            item.knockBack = 2.25f;
            item.mana = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 32;
            item.useTime = 32;
            item.UseSound = SoundID.Item9;
            item.rare = ItemRarityID.LightRed;
            item.value = AQItem.OmegaStariteWeaponValue;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<MagicBolt>();
            item.autoReuse = true;
        }
    }

    public class MagicBolt : ModProjectile
    {
        public override string Texture => AQMod.ModName + "/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            var center = projectile.Center;
            if (Main.myPlayer == projectile.owner)
            {
                if (projectile.ai[0] == 0f)
                {
                    projectile.ai[0] = 1f;
                    projectile.timeLeft = (int)((center - Main.MouseWorld) / projectile.velocity.Length()).Length();
                }
            }
            int d = Dust.NewDust(center, 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(255, 150, 150, 0), 1.5f);
            Main.dust[d].velocity = Vector2.Normalize(projectile.velocity.RotatedBy(MathHelper.PiOver2)) * (float)Math.Sin(projectile.timeLeft * 0.75f) * 2f;
        }

        public override void Kill(int timeLeft)
        {
            var center = projectile.Center;
            int type = ModContent.DustType<MonoDust>();
            var clr = new Color(255, 150, 150, 0);
            int d = Dust.NewDust(center, 2, 2, type, 0f, 0f, 0, clr, 1.5f);
            float sum = projectile.position.X + projectile.position.Y;
            Main.dust[d].velocity = Vector2.Zero;
            int x = (int)(center.X / 16f);
            int y = (int)(center.Y / 16f);
            const int maxHeight = 6;
            int topY = -maxHeight;
            int bottomY = maxHeight;
            for (int i = 0; i < maxHeight; i++)
            {
                if (AQWorldGen.ActiveAndSolid(x, y - maxHeight + i))
                {
                    topY = -maxHeight + i;
                }
                if (AQWorldGen.ActiveAndSolid(x, y + maxHeight - i))
                {
                    bottomY = maxHeight - i;
                }
            }
            float x2 = x * 16f;
            float x3 = x * 16f + 8f;
            Main.PlaySound(SoundID.Dig, projectile.Center);
            for (int yy = y + topY; yy < y + bottomY; yy++)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(new Vector2(x2, yy * 16f), 16, 16, type, 0, 0, 0, clr, 2f);
            }
            type = ModContent.ProjectileType<MagicPillar>();
            int height = (y + bottomY - (y + topY)) * 16;
            int p = Projectile.NewProjectile(x3, (y + topY) * 16f, 20f, 0f, type, projectile.damage, projectile.knockBack, projectile.owner);
            Main.projectile[p].height = height;
            p = Projectile.NewProjectile(x3, (y + topY) * 16f, -20f, 0f, type, projectile.damage, projectile.knockBack, projectile.owner);
            Main.projectile[p].height = height;
        }
    }

    public class MagicPillar : ModProjectile
    {
        public override string Texture => AQMod.ModName + "/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 30;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            int type = ModContent.DustType<MonoDust>();
            var clr = new Color(255, 150, 150, 0);
            Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0, 0, 0, clr, 2f);
            Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0, 0, 0, clr, 2f);
        }
    }
}