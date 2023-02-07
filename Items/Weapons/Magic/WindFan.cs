using Aequus.Items.Misc.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    public class WindFan : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ProjectileID.BallofFrost, 40, 12f, hasAutoReuse: true);
            Item.SetWeaponValues(50, 10f, bonusCritChance: 21);
            Item.rare = ItemDefaults.RarityDustDevil;
            Item.value = ItemDefaults.ValueDustDevil;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FlowerofFire)
                .AddIngredient(ItemID.FlowerofFrost)
                .AddIngredient<AtmosphericEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.SkyFracture);
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}