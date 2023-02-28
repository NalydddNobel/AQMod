using Aequus.Items.Accessories.Passive;
using Aequus.Content.Boss.DustDevil;
using Terraria.ID;
using Terraria.ModLoader;
using Aequus.Items;

namespace Aequus.Content.Boss.DustDevil.Rewards
{
    public class DustDevilBag : TreasureBagBase
    {
        protected override int InternalRarity => ItemRarityID.LightPurple;
        protected override bool PreHardmode => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .Add<Stormcloak>(chance: 1, stack: 1)
                .Coins<DustDevil>();
        }
    }
}