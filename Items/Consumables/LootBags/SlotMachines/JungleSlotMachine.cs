using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags.SlotMachines
{
    public class JungleSlotMachine : SlotMachineBase
    {
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .AddRouletteItem(ItemID.FeralClaws)
                .AddRouletteItem(ItemID.AnkletoftheWind)
                .AddRouletteItem(ItemID.StaffofRegrowth)
                .AddRouletteItem(ItemID.Boomstick)
                .AddRouletteItem(ItemID.FlowerBoots)
                .AddRouletteItem(ItemID.FiberglassFishingPole)
                .AddRouletteItem(ItemID.Seaweed)
                .AddRouletteItem(ItemID.LivingMahoganyLeafWand, 4)
                .AddSpecialRouletteItem(ItemID.LivingMahoganyWand, ItemID.LivingMahoganyLeafWand)
                .AddRouletteItem(ItemID.BeeMinecart, 4)
                .Add(ItemID.HoneyDispenser, chance: 4, stack: 1)
                .Add(ItemID.SilverCoin, chance: 1, stack: (50, 80));
            ModifyItemLoot_AddCommonDrops(itemLoot);
        }
    }
}