using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Vanity
{
    public class FishyFins : ModItem, IUpdateVanity
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 10;
            item.accessory = true;
            item.rare = AQItem.Rarities.DedicatedItem;
            item.value = Item.sellPrice(silver: 20);
            item.vanity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            item.color = Main.LocalPlayer.skinColor;
            return null;
        }

        void IUpdateVanity.UpdateVanitySlot(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i)
        {
            drawEffects.headAcc = PlayerHeadAccID.FishyFins;
            drawEffects.cHeadAcc = player.dye[i % AQPlayer.MaxDye].dye;
        }
    }
}