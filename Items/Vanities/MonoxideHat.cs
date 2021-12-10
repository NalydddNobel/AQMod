using AQMod.Assets.LegacyItemOverlays;
using AQMod.Common.Graphics.PlayerEquips;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities
{
    public class MonoxideHat : ModItem, IUpdateEquipVisuals
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(AQUtils.GetPath(this) + "_Glow"), item.type);
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

        void IUpdateEquipVisuals.UpdateEquipVisuals(Player player, AQPlayer drawingPlayer, int i)
        {
            drawingPlayer.headAcc = (int)PlayerHeadOverlayID.MonoxideHat;
            drawingPlayer.cHeadAcc = player.dye[i % AQPlayer.DYE_WRAP].dye;
        }
    }
}