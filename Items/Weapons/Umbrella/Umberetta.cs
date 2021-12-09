using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Umbrella
{
    public class Umberetta : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 44;
            item.ranged = true;
            item.useTime = 18;
            item.useAnimation = 18;
            item.width = 30;
            item.height = 20;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = AQItem.Rarities.GaleStreamsRare + 1;
            item.value = AQItem.Prices.GaleStreamsValue;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.useAmmo = ItemID.WoodenArrow;
            item.shootSpeed = 20f;
            item.UseSound = SoundID.Item5;
            item.noMelee = true;
            item.knockBack = 6.5f;
            item.autoReuse = true;
        }
    }
}