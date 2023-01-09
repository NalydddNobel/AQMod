using Aequus.Items.Misc.Energies;
using Aequus.Items.Misc.Materials;
using Aequus.Projectiles.Summon.CandleSpawners;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Candles
{
    public class DragonsBreath : SoulCandleBase
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            DefaultToCandle<DragonsBreathProj>(120);
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 10);
            Item.flame = true;
            Item.UseSound = SoundID.Item83;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += -4f * player.direction;
            player.itemLocation.Y += 0f;

            Lighting.AddLight(player.itemLocation, Color.LightCyan.ToVector3() * Main.rand.NextFloat(0.5f, 0.8f));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DungeonCandle>()
                .AddIngredient<Hexoplasm>(8)
                .AddIngredient<UltimateEnergy>(3)
                .AddTile(TileID.LunarCraftingStation)
                .TryRegisterAfter(ItemID.StardustDragonStaff);
        }
    }
}