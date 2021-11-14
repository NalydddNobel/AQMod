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
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.MagicWand>();
            item.autoReuse = true;
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