namespace AequusRemake.Core.ContentGeneration;

internal class InstancedSandBallItem : InstancedTileItem {
    [CloneByReference]
    protected ModProjectile _sandBallProjectile;
    protected readonly int _bonusDamage;

    public InstancedSandBallItem(ModTile parent, int bonusSandGunDamage) : base(parent, researchSacrificeCount: 100) {
        _bonusDamage = bonusSandGunDamage;
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ItemSets.SandgunAmmoProjectileData[Type] = new(_sandBallProjectile.Type, _bonusDamage);
    }

    public void SetProjectile(ModProjectile sandGunProjectile) {
        _sandBallProjectile = sandGunProjectile;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Item.ammo = AmmoID.Sand;
        Item.notAmmo = true;
    }
}
