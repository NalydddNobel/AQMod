using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.SteelPlate
{
    [AutoloadEquip(EquipType.Head)]
    public class SteelPlatedAHelmet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 8;
            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(gold: 20);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SteelPlatedChestplate>() && legs.type == ModContent.ItemType<SteelPlatedGreaves>();
        }
    }
}