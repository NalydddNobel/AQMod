using Aequus.Items;
using Aequus.Items.Accessories.Combat.Passive;
using Aequus.Items.Vanity.Equipable.Masks;
using Aequus.Items.Weapons.Magic.Gamestar;
using Aequus.Items.Weapons.Ranged.Guns.Raygun;
using Aequus.Items.Weapons.Summon.ScribbleNotebook;
using Aequus.NPCs.Monsters.BossMonsters.OmegaStarite;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.TreasureBag {
    public class OmegaStariteBag : TreasureBagBase {
        protected override int InternalRarity => ItemRarityID.LightRed;
        protected override bool PreHardmode => true;

        public override void ModifyItemLoot(ItemLoot itemLoot) {
            this.CreateLoot(itemLoot)
                .Add<CelesteTorus>(chance: 1, stack: 1)
                .Add<OmegaStariteMask>(chance: 7, stack: 1)
                .AddOptions(chance: 1, ModContent.ItemType<Raygun>(), ModContent.ItemType<Gamestar>(), ModContent.ItemType<ScribbleNotebook>())
                .Coins<OmegaStarite>();
        }
    }
}