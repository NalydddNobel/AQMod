using Aequus.Common.Items;
using Aequus.Core.Initialization;

namespace Aequus.Old.Content.Necromancy.Sceptres.Dungeon;

[AutoloadGlowMask]
public class Revenant : ScepterBase {
    public override void SetDefaults() {
        Item.DefaultToNecromancy(25);
        Item.SetWeaponValues(40, 1f, 0);
        Item.shoot = ModContent.ProjectileType<RevenantProj>();
        Item.shootSpeed = 6.5f;
        Item.rare = ItemCommons.Rarity.DungeonLoot;
        Item.value = ItemCommons.Price.DungeonLoot;
        Item.mana = 15;
        Item.UseSound = SoundID.Item8;
    }
}