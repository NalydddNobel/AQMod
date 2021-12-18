using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class SteelPlatedChestplate : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 15;
            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(gold: 25);
        }
    }
}