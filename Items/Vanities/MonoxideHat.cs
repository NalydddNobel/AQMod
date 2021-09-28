using AQMod.Assets.Enumerators;
using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Common;
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
                ItemOverlayLoader.Register(new Glowmask(GlowID.MonoxideHatItem), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 10;
            item.accessory = true;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(silver: 20);
            item.vanity = true;
        }

        void IUpdateEquipVisuals.UpdateEquipVisuals(Player player, GraphicsPlayer drawingPlayer, int i)
        {
            drawingPlayer.headOverlay = (int)PlayerHeadOverlayID.MonoxideHat;
            drawingPlayer.cHeadOverlay = player.dye[i % AQPlayer.DyeWrap].dye;
        }
    }
}