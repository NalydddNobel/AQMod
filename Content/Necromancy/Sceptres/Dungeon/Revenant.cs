using Aequus.Common.Items;
using Aequus.Items;
using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Sceptres.Dungeon;

namespace Aequus.Content.Necromancy.Sceptres.Dungeon;

[AutoloadGlowMask]
public class Revenant : ScepterBase {
    public override void SetDefaults() {
        Item.DefaultToNecromancy(25);
        Item.SetWeaponValues(40, 1f, 0);
        Item.shoot = ModContent.ProjectileType<RevenantProj>();
        Item.shootSpeed = 6.5f;
        Item.rare = ItemDefaults.RarityDungeon;
        Item.value = ItemDefaults.ValueDungeon;
        Item.mana = 15;
        Item.UseSound = SoundID.Item8;
    }
}