using Aequu2.Core;
using tModLoaderExtended.GlowMasks;

namespace Aequu2.Old.Content.Necromancy.Sceptres.Dungeon;

[AutoloadGlowMask]
public class Revenant : ScepterBase {
    public override void SetDefaults() {
        Item.DefaultToNecromancy(25);
        Item.SetWeaponValues(40, 1f, 0);
        Item.shoot = ModContent.ProjectileType<RevenantProj>();
        Item.shootSpeed = 6.5f;
        Item.rare = Commons.Rare.BiomeDungeon;
        Item.value = Commons.Cost.BiomeDungeon;
        Item.mana = 15;
        Item.UseSound = SoundID.Item8;
    }
}