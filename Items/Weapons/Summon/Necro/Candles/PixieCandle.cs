using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Summon.CandleSpawners;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Weapons.Summon.Necro.Candles
{
    public class PixieCandle : SoulCandleBase
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            DefaultToCandle<FallenAngelProj>(120);
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
            Item.flame = true;
            Item.UseSound = SoundID.Item83;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += -4f * player.direction;
            player.itemLocation.Y += 8f;

            Lighting.AddLight(player.itemLocation, TorchID.Torch);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Candle)
                .AddIngredient(ItemID.PixieDust, 50)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.DemonAltar)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumCandle)
                .AddIngredient(ItemID.PixieDust, 50)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}