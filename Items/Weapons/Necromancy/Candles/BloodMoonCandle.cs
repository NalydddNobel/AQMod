using Aequus.Common;
using Aequus.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Weapons.Necromancy.Candles {
    [WorkInProgress]
    public class BloodMoonCandle : SoulCandleBase {
        public override void SetDefaults() {
            DefaultToCandle(20);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
            Item.flame = true;
            Item.UseSound = SoundID.Item83;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame) {
            player.itemLocation.X += -4f * player.direction;
            player.itemLocation.Y += 8f;

            Lighting.AddLight(player.itemLocation, Color.Red.ToVector3() * Main.rand.NextFloat(0.5f, 0.8f));
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient(ItemID.Candle)
                .AddIngredient<BloodyTearstone>(6)
                .AddTile(TileID.DemonAltar)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumCandle)
                .AddIngredient<BloodyTearstone>(6)
                .AddTile(TileID.DemonAltar)
                .Register();
#endif
        }
    }
}