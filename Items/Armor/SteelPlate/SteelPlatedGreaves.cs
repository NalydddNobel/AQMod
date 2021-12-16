using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.SteelPlate
{
    [AutoloadEquip(EquipType.Legs)]
    public class SteelPlatedGreaves : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 8;
            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(gold: 20);
        }
    }
}