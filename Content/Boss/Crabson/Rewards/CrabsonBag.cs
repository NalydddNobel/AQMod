using Aequus.Items;
using Aequus.Items.Tools;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.Crabson.Rewards
{
    public class CrabsonBag : TreasureBagBase
    {
        protected override int InternalRarity => ItemRarityID.Blue;
        protected override bool PreHardmode => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .Add<Crabax>(chance: 1, stack: 1)
                .Add<CrabsonMask>(chance: 7, stack: 1)
                .Coins<Crabson>();
        }
    }
}