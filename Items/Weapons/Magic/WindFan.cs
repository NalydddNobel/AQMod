using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    public class WindFan : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<WindFanProj>(), 40, 12f, hasAutoReuse: true);
            Item.SetWeaponValues(50, 10f, bonusCritChance: 21);
            Item.rare = ItemDefaults.RarityDustDevil;
            Item.value = ItemDefaults.DustDevilValue;
            Item.channel = true;
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