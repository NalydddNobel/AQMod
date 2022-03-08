using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public sealed class Skrawler : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            this.Glowmask(() => new Color(200, 200, 200, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0.9f, 1f));
        }

        public override void SetDefaults()
        {
            item.damage = 48;
            item.magic = true;
            item.useTime = 48;
            item.useAnimation = 48;
            item.width = 32;
            item.height = 32;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = AQItem.RarityDemonSiege;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.Skrawler>();
            item.shootSpeed = 10f;
            item.mana = 12;
            item.autoReuse = true;
            item.UseSound = SoundID.Item20;
            item.value = AQItem.Prices.DemonSiegeWeaponValue;
            item.knockBack = 2f;
        }
    }
}