using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class ATM : ModItem, IUpdatePlayerSafe
    {
        public override string Texture => "Terraria/Item_" + ItemID.Safe;

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.MoneyTrough);
            item.UseSound = SoundID.Item97;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<Projectiles.Pets.PetSafe>();
        }

        void IUpdatePlayerSafe.UpdatePlayerSafe(Player player, int i)
        {
            var aQPlr = player.GetModPlayer<AQPlayer>();
            var type = ModContent.ProjectileType<Projectiles.Pets.PetSafe>();
            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(player.Center, new Vector2(0f, 0f), type, 0, 0f, player.whoAmI);
            for (int j = 0; j < Main.maxProjectiles; j++)
            {
                if (Main.projectile[j].active && Main.projectile[j].type == type)
                    Main.projectile[j].timeLeft = 30;
            }
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Safe);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddTile(TileID.DemonAltar);
            r.SetResult(this);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.PiggyBank);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddTile(TileID.DemonAltar);
            r.SetResult(ItemID.MoneyTrough);
            r.AddRecipe();
        }
    }
}