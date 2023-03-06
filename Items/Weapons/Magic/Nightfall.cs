using Aequus.Common.Recipes;
using Aequus.Items.Accessories.Offense;
using Aequus.Items.Weapons.Summon.Minion;
using Aequus.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    [LegacyName("WowHat")]
    public class Nightfall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(9, 1f);
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<NightfallProj>(), 24, 12f, true);
            Item.mana = 10;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemDefaults.RarityGlimmer;
            Item.value = ItemDefaults.ValueGlimmer;
            Item.UseSound = SoundID.Item8;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<StariteStaff>());
        }
    }
}