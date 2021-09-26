using AQMod.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee.Flails
{
    public class JerryClawFlail : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 32;
            item.melee = true;
            item.useTime = 30;
            item.useAnimation = 30;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.Flails.JerryClaw>();
            item.shootSpeed = 13.5f;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.CrabsonWeaponValue;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 6f;
        }
    }
}