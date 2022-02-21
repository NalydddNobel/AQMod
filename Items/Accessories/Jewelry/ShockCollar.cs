using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Jewelry
{
    [AutoloadEquip(EquipType.Neck)]
    public class ShockCollar : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().shockCollar = true;
            player.allDamageMult += 0.05f;
            if (player.wet)
            {
                player.meleeCrit += 10;
                player.rangedCrit += 10;
                player.magicCrit += 10;
                player.thrownCrit += 10;
            }
        }
    }
}