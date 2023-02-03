using Aequus.Items.Accessories;
using Aequus.NPCs.Boss.DustDevil;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Boss
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