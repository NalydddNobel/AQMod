using AQMod.Assets.LegacyItemOverlays;
using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Vanity
{
    public class MonoxideHat : ModItem, IUpdateEquipVisuals
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyGlowmaskOverlay(this.GetPath() + "_Glow"), item.type);
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

        void IUpdateEquipVisuals.UpdateEquipVisuals(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i)
        {
            drawEffects.headAcc = PlayerHeadAccID.MonoxideHat;
            drawEffects.cHeadAcc = player.dye[i % AQPlayer.MaxDye].dye;
        }
    }
}