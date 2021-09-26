using AQMod.Assets.Enumerators;
using AQMod.Common;
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
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 20);
            item.vanity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            item.color = Main.LocalPlayer.skinColor;
            return null;
        }

        void IUpdateEquipVisuals.UpdateEquipVisuals(Player player, AQVisualsPlayer drawingPlayer, int i)
        {
            drawingPlayer.headOverlay = (int)PlayerHeadOverlayID.FishyFins;
            drawingPlayer.cHeadOverlay = player.dye[i % AQPlayer.DyeWrap].dye;
        }
    }
}