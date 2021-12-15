using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class StriderPalms : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 4;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 80);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<AQPlayer>().striderPalms = true;
            player.minionDamage += 0.1f;
        }
    }
}