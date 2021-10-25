using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Powders
{
    public class FertilePowder : ModItem
    {
        public override void SetDefaults()
        {
			item.damage = 0;
			item.useStyle = 1;
			item.shootSpeed = 12f;
			item.shoot = ModContent.ProjectileType<Projectiles.FertilePowder>();
			item.width = 16;
			item.height = 24;
			item.maxStack = 99;
			item.consumable = true;
			item.UseSound = SoundID.Item1;
			item.useAnimation = 15;
			item.useTime = 15;
			item.noMelee = true;
			item.value = Item.sellPrice(silver: 1);
			item.rare = ItemRarityID.Green;
        }
    }
}