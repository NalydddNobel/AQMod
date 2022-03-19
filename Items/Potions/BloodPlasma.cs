using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions
{
    public class BloodPlasma : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 5);
            item.rare = ItemRarityID.Green;
            item.accessory = true;
            item.UseSound = SoundID.Item3;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.healLife = 100;
            item.consumable = true;
            item.maxStack = 30;
            item.potion = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (item.stack > 1)
            {
                player.QuickSpawnClonedItem(item, item.stack - 1);
                item.stack = 1;
            }
            player.GetModPlayer<AQPlayer>().healEquip = item;
        }
    }
}