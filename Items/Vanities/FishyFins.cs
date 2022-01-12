using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities
{
    public class FishyFins : ModItem, IUpdateEquipVisuals
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 10;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(silver: 20);
            item.vanity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            item.color = Main.LocalPlayer.skinColor;
            return null;
        }

        void IUpdateEquipVisuals.UpdateEquipVisuals(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i)
        {
            drawEffects.headAcc = (int)PlayerHeadAccID.FishyFins;
            drawEffects.cHeadAcc = player.dye[i % AQPlayer.MaxDye].dye;
        }
    }
}