using Aequus.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.LegendaryFish {
    public class GoreFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 2;
            AequusItem.LegendaryFishIDs.Add(Type);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .Add(ItemID.LavaFishingHook, chance: 1, stack: 1);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Batfish);
            Item.questItem = false;
        }
    }
}