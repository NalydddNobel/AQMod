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
            this.Glowmask();
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.magic = true;
            item.damage = 32;
            item.knockBack = 2.25f;
            item.mana = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 32;
            item.useTime = 32;
            item.UseSound = SoundID.Item8;
            item.rare = ItemRarityID.LightRed;
            item.value = AQItem.Prices.OmegaStariteWeaponValue;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.MagicWandProj>();
            item.autoReuse = true;
        }
    }
}