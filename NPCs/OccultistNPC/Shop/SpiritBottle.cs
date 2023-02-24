using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.OccultistNPC.Shop
{
    public class SpiritBottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

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