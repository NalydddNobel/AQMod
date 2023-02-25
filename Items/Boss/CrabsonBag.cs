using Aequus.Items.Vanity.Masks;
using Aequus.Items.Tools;
using Aequus.NPCs.Boss.Crabson;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Boss
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