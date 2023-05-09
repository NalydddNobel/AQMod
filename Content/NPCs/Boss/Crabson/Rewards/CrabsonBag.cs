using Aequus.Items;
using Aequus.Items.Consumables.Permanent;
using Aequus.Items.Weapons.Ranged.Misc;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.NPCs.Boss.Crabson.Rewards {
    public class CrabsonBag : TreasureBagBase {
        protected override int InternalRarity => ItemRarityID.Blue;
        protected override bool PreHardmode => true;

        public override void ModifyItemLoot(ItemLoot itemLoot) {
            this.CreateLoot(itemLoot)
                .Add<MoneyTrashcan>(chance: 1, stack: 1)
                .Add<CrabsonMask>(chance: 7, stack: 1)
                .AddOptions(chance: 1, ModContent.ItemType<JunkJet>())
                .Coins<Crabson>();
        }
    }
}