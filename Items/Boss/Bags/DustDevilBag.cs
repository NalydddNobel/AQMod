using Aequus.Items.Accessories;
using Aequus.Items.Misc.Energies;
using Aequus.NPCs.Boss;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Boss.Bags
{
    public class DustDevilBag : TreasureBagBase
    {
        protected override int InternalRarity => ItemRarityID.LightPurple;
        protected override bool PreHardmode => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .Add<Stormcloak>(chance: 1, stack: 1)
                .Add<AtmosphericEnergy>(chance: 1, stack: 3)
                .Coins<DustDevil>();
        }
    }
}