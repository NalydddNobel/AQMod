using AQMod.Common.Graphics;
using AQMod.Dusts;
using AQMod.Effects;
using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public sealed class FizzlingFire : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            this.Glowmask(() => new Color(200, 200, 200, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0.9f, 1f));
        }

        public override void SetDefaults()
        {
            item.damage = 25;
            item.magic = true;
            item.useTime = 2;
            item.useAnimation = 20;
            item.width = 40;
            item.height = 40;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = AQItem.Rarities.GoreNestRare;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.FizzlingFire>();
            item.shootSpeed = 11.5f;
            item.mana = 30;
            item.autoReuse = true;
            item.UseSound = SoundID.Item20;
            item.value = AQItem.Prices.DemonSiegeWeaponValue;
            item.knockBack = 2f;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff(BuffID.ManaSickness);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (AQConfigClient.c_TonsofScreenShakes)
            {
                FX.SetShake(2 * AQConfigClient.c_EffectIntensity);
            }
            int randcount = 3 + Main.rand.Next(4);
            var velo = new Vector2(speedX, speedY);
            position += Vector2.Normalize(velo) * (item.width * 1.5f);
            for (int i = 0; i < randcount; i++)
            {
                var velo2 = velo.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f) + AQUtils.Wave(AQGraphics.TimerBasedOnTimeOfDay * 12f, -0.1f, 0.1f));
                int p = Projectile.NewProjectile(position + velo2 * Main.rand.NextFloat(0.5f, 3f), velo2, type, damage, knockBack, player.whoAmI);
                Main.projectile[p].scale = Main.rand.NextFloat(0.5f, 1.65f);
                Main.projectile[p].timeLeft = Main.rand.Next(30, 45);
                if (Main.projectile[p].timeLeft >= 40)
                {
                    Main.projectile[p].scale *= 0.3f;
                }
                for (int j = 0; j < 5; j++)
                {
                    int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, ModContent.DustType<MonoDust>());
                    var diff = Main.dust[d].position - Main.projectile[p].Center;
                    float dustIntensity = diff.Length() / Main.projectile[p].width;
                    Main.dust[d].color = new Color(0.75f * dustIntensity, 0.75f * dustIntensity, 1f * dustIntensity, 0f);
                    Main.dust[d].scale = dustIntensity * 2f;
                    Main.dust[d].velocity = Vector2.Normalize(Main.dust[d].velocity) * 16f * dustIntensity;
                }
            }
            return true;
        }
    }
}