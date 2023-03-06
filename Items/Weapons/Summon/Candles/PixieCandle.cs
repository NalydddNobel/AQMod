using Aequus.Common.Recipes;
using Aequus.Projectiles.Summon.CandleSpawners;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Candles
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
            Item.value = Item.sellPrice(gold: 2);
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
            AequusRecipes.CreateShimmerTransmutation(ModContent.ItemType<OccultistCandle>(), Type, AequusRecipes.ShimmerConditionHackHardmode);
        }
    }
}