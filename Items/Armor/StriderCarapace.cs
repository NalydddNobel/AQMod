using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class StriderCarapace : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 3;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 80);
        }
    }
}