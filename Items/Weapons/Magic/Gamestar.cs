using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    public class Gamestar : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.width = 20;
            Item.height = 20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.shoot = ModContent.ProjectileType<GamestarProj>();
            Item.shootSpeed = 25f;
            Item.mana = 8;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item75.WithPitch(1f);
            Item.value = ItemDefaults.OmegaStariteValue;
            Item.knockBack = 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpaceGun)
                .AddIngredient<CosmicEnergy>()
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.Anvils)
                .RegisterBefore(ItemID.SpaceGun);
        }
    }
}