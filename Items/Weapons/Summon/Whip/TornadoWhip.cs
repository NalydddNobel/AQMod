using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Summon.Whip;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Whip
{
    public class TornadoWhip : ModItem
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
            Item.DefaultToWhip(ModContent.ProjectileType<TornadoWhipProj>(), 33, 5f, 4f);
            Item.rare = ItemDefaults.RarityDustDevil;
            Item.value = ItemDefaults.DustDevilValue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FireWhip)
                .AddIngredient(ItemID.CoolWhip)
                .AddIngredient<AtmosphericEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.CoolWhip);
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}