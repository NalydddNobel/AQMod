using Aequus.Items.Armor.Vanity;
using Aequus.Items.Boss.Expert;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.LootBags;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Weapons.Melee;
using Aequus.NPCs.Boss;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Boss.Bags
{
    public class OmegaStariteBag : TreasureBagBase
    {
        protected override int InternalRarity => ItemRarityID.LightRed;
        protected override bool PreHardmode => false;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .Add<CelesteTorus>(chance: 1, stack: 1)
                .Add<OmegaStariteMask>(chance: 7, stack: 1)
                .Add<UltimateSword>(chance: 1, stack: 1)
                .AddOptions(chance: 1, ModContent.ItemType<ScrollDye>(), ModContent.ItemType<OutlineDye>())
                .Add<CosmicEnergy>(chance: 1, stack: 3)
                .Add<AstralCookie>(chance: 1, stack: 1)
                .Coins<OmegaStarite>();
        }
    }
}