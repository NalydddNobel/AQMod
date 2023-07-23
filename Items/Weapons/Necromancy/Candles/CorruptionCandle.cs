using Aequus.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Weapons.Necromancy.Candles {
    [WorkInProgress]
    public class CorruptionCandle : SoulCandleBase {
        public override void SetDefaults() {
            DefaultToCandle(14);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
            Item.flame = true;
            Item.UseSound = SoundID.Item83;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame) {
            player.itemLocation.X += -4f * player.direction;
            player.itemLocation.Y += 8f;

            Lighting.AddLight(player.itemLocation, Color.Violet.ToVector3() * Main.rand.NextFloat(0.5f, 0.8f));
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(Type, 3)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}