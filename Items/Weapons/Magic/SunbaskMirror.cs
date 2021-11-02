using AQMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class SunbaskMirror : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 76;
            item.magic = true;
            item.useTime = 38;
            item.useAnimation = 38;
            item.width = 50;
            item.height = 50;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.MoonMoonMirror>();
            item.shootSpeed = 24.11f;
            item.mana = 11;
            item.autoReuse = true;
            item.UseSound = SoundID.Item101;
            item.value = AQItem.AtmosphericCurrentsValue;
            item.knockBack = 6f;
            item.channel = true;
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            return Main.dayTime;
        }
    }
}