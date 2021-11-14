using AQMod.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class CinnabarBow : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 22;
            item.ranged = true;
            item.useTime = 26;
            item.useAnimation = 26;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Blue;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 8.5f;
            item.useAmmo = AmmoID.Arrow;
            item.UseSound = SoundID.Item5;
            item.value = AQItem.CrabsonWeaponValue;
            item.noMelee = true;
            item.knockBack = 6.5f;
        }
    }
}