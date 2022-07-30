using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags.Roulettes
{
    public class GlowingMushroomsRoulette : GoldenRoulette
    {
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            base.ModifyItemLoot(itemLoot);
            this.CreateLoot(itemLoot)
                .AddRouletteItem(ItemID.Shroomerang)
                .AddRouletteItem(ItemID.MushroomHat)
                .AddSpecialRouletteItem(ItemID.MushroomVest, ItemID.MushroomHat)
                .AddSpecialRouletteItem(ItemID.MushroomPants, ItemID.MushroomHat)
                .AddRouletteItem(ItemID.ShroomMinecart, 8);
        }
    }
}