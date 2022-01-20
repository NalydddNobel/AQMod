using AQMod.Content.HookBarbs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class CrabBarb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 15;
            item.knockBack = 0f;
            item.crit = 4;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 40);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var barbPlayer = player.GetModPlayer<HookBarbPlayer>();
            barbPlayer.AddBarb(new CrabBarbAttachment(item));
        }
    }
}