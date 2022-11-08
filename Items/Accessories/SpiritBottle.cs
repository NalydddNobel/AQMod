using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class SpiritBottle : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.PygmyNecklace);
            Item.neckSlot = 0;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().ghostSlotsMax++;
        }
    }
}