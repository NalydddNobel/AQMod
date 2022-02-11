using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.HookUpgrades
{
    public class Meathook : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = AQItem.Prices.DemonSiegeWeaponValue;
            item.rare = ItemRarityID.LightRed;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().meathook = true;
        }
    }
}