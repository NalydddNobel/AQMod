using Aequus.Common;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Weapons.Necromancy.Candles {
    [WorkInProgress]
    public class DragonsBreath : SoulCandleBase {
        public override void SetDefaults() {
            DefaultToCandle(120);
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 10);
            Item.flame = true;
            Item.UseSound = SoundID.Item83;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame) {
            player.itemLocation.X += -4f * player.direction;
            player.itemLocation.Y += 0f;

            Lighting.AddLight(player.itemLocation, Color.LightCyan.ToVector3() * Main.rand.NextFloat(0.5f, 0.8f));
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient<DungeonCandle>()
                .AddIngredient<Hexoplasm>(8)
                .AddIngredient<UltimateEnergy>(3)
                .AddTile(TileID.LunarCraftingStation)
                .TryRegisterAfter(ItemID.StardustDragonStaff);
#endif
        }
    }
}