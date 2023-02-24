using Aequus.Items.Accessories.Fishing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Fishing.LegendaryFish
{
    public class ArgonFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 2;
            AequusItem.LegendaryFishIDs.Add(Type);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .Add<DevilsTongue>(chance: 1, stack: 1)
                .Add(ItemID.ArgonMoss, chance: 1, stack: (10, 25));
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Batfish);
            Item.questItem = false;
        }
    }
}