using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags.SlotMachines
{
    public class SnowRoulette : SlotMachineBase
    {
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .AddRouletteItem(ItemID.IceBoomerang)
                .AddRouletteItem(ItemID.IceBlade)
                .AddRouletteItem(ItemID.IceSkates)
                .AddRouletteItem(ItemID.SnowballCannon)
                .AddRouletteItem(ItemID.BlizzardinaBottle)
                .AddRouletteItem(ItemID.FlurryBoots)
                .AddRouletteItem(ItemID.Fish)
                .AddRouletteItem(ItemID.IceMirror, 4)
                .Add(ItemID.IceMachine, chance: 4, stack: 1)
                .Add(ItemID.Extractinator, chance: 8, stack: 1)
                .Add(ItemID.SilverCoin, chance: 1, stack: (50, 80));
            ModifyItemLoot_AddCommonDrops(itemLoot);
        }
    }
}