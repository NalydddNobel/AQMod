using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.CrabSeason
{
    [AutoloadEquip(EquipType.Head)]
    public class HermitShell : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 2;
            item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            float speed = player.velocity.Length();
            if (speed < 5f)
            {
                player.statDefense += (int)((5f - speed) * 2);
            }
        }
    }
}