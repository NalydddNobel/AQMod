using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee.Dagger
{
    public class CrystalDagger : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.melee = true;
            item.damage = 19;
            item.knockBack = 2f;
            item.useAnimation = 8;
            item.useTime = 8;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(silver: 75);
            item.shootSpeed = 8f;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.CrystalDagger>();
            item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < 1;
        }
    }
}