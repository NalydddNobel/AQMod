using AQMod.Common.Graphics.PlayerEquips;
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

        void IUpdateEquipVisuals.UpdateEquipVisuals(Player player, AQPlayer drawingPlayer, int i)
        {
            drawingPlayer.headAcc = (int)PlayerHeadOverlayID.FishyFins;
            drawingPlayer.cHeadAcc = player.dye[i % AQPlayer.DYE_WRAP].dye;
        }
    }
}