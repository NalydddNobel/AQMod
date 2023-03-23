using Aequus.Items;
using Aequus.Items.Accessories.Passive;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Melee.Thrown;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.DustDevil.Rewards
{
    public class DustDevilBag : TreasureBagBase
    {
        protected override int InternalRarity => ItemRarityID.LightPurple;
        protected override bool PreHardmode => true;

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .Add<Stormcloak>(chance: 1, stack: 1)
                .AddOptions(chance: 1, ModContent.ItemType<PhaseDisc>(), ModContent.ItemType<WindFan>())
                .Coins<DustDevil>();
        }
    }
}