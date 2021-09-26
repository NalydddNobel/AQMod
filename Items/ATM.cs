using AQMod.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items
{
    public class ATM : ModItem, IUpdatePlayerSafe
    {
        public override string Texture => "Terraria/Item_" + ItemID.Safe;

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.MoneyTrough);
            item.UseSound = SoundID.Item97;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<Projectiles.ATM>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            return player.altFunctionUse != 2;
        }

        void IUpdatePlayerSafe.UpdatePlayerSafe(Player player, int i)
        {
            var aQPlr = player.GetModPlayer<AQPlayer>();
            var type = ModContent.ProjectileType<Projectiles.ATM>();
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
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 3);
            r.AddTile(TileID.DemonAltar);
            r.SetResult(this);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.PiggyBank);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 3);
            r.AddTile(TileID.DemonAltar);
            r.SetResult(ItemID.MoneyTrough);
            r.AddRecipe();
        }
    }
}