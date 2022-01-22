using Terraria;
using Terraria.ID;
using Terraria.Localization;
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

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ItemID.VikingHelmet;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.AQMod.ArmorSetBonus.SkyrimArmor");
            player.endurance += 0.05f;
        }
    }
}