using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Fishing
{
    public class Nimrod : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WoodFishingPole);
            item.fishingPole = 45;
            item.shootSpeed = 10f;
            item.rare = ItemRarityID.LightPurple;
            item.shoot = ModContent.ProjectileType<Projectiles.Fishing.NimrodElectric>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<Projectiles.Fishing.Nimrod>();
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
    }
}