using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags
{
    public class CrabCreviceCrateHard : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 10;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            var l = new ItemLoot(ItemID.OceanCrate, Main.ItemDropsDB).Get(includeGlobalDrops: false);
            foreach (var loot in l)
            {
                itemLoot.Add(loot);
            }
            this.CreateLoot(itemLoot);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.OceanCrateHard);
        }

        public override bool CanRightClick()
        {
            return true;
        }
    }
}