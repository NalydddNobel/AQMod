using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class Dysesthesia : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 16;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.melee = true;
            item.damage = 21;
            item.knockBack = 8f;
            item.value = AQItem.Prices.DemonSiegeWeaponValue;
            item.UseSound = SoundID.Item1;
            item.rare = AQItem.Rarities.GoreNestRare;
            item.channel = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shootSpeed = 18f;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.Dysesthesia>();
        }
    }
}