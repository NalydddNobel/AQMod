using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.Crab
{
    [AutoloadEquip(EquipType.Head)]
    public class StriderCarapace : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 5;
            item.rare = ItemRarityID.Blue;
        }
    }
}