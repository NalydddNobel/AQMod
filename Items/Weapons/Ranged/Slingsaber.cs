using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Ranged;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class Slingsaber : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.width = 32;
            Item.height = 24;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<SlingsaberProj>();
            Item.noUseGraphic = true;
            Item.shootSpeed = 12f;
            Item.UseSound = new SoundStyle("Aequus/Sounds/Items/Slingshot/stretch") { Volume = 0.2f, };
            Item.value = Item.sellPrice(gold: 6);
            Item.knockBack = 1f;
            Item.useAmmo = SlingshotAmmos.BirdAmmo;
            Item.channel = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Slingshot>()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.SoulofSight, 20)
                .AddIngredient<AtmosphericEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}