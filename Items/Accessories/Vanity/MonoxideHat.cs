using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Vanity
{
    public class MonoxideHat : ModItem, IUpdateVanity
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask();
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 10;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(silver: 20);
            item.vanity = true;
        }

        void IUpdateVanity.UpdateVanitySlot(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i)
        {
            drawEffects.headAcc = PlayerHeadAccID.MonoxideHat;
            drawEffects.cHeadAcc = player.dye[i % AQPlayer.MaxDye].dye;
        }
    }
}