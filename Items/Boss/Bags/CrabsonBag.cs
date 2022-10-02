using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Healing;
using Aequus.Items.Armor.Vanity;
using Aequus.Items.Consumables.LootBags;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Tools;
using Aequus.NPCs.Boss;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Boss.Bags
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
                .Add<AquaticEnergy>(chance: 1, stack: 3)
                .AddOptions(chance: 1, ModContent.ItemType<Mendshroom>(), ModContent.ItemType<AmmoBackpack>())
                .Coins<Crabson>();
        }
    }
}