using Aequus.Items.Accessories.Fishing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Fish.Legendary
{
    public class KryptonFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 2;
            AequusItem.LegendaryFishIDs.Add(Type);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .Add<Ramishroom>(chance: 1, stack: 1)
                .Add(ItemID.KryptonMoss, chance: 1, stack: (10, 25));
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 1);
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Quest;
            Item.questItem = true;
        }
    }
}