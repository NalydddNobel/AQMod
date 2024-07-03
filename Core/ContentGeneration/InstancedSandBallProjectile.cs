namespace Aequu2.Core.ContentGeneration;

internal class InstancedSandBallProjectile : InstancedModProjectile {
    [CloneByReference]
    protected readonly ModTile _tile;
    [CloneByReference]
    protected readonly ModItem _item;
    protected readonly bool _friendly;

    public InstancedSandBallProjectile(ModTile sandTile, ModItem sandItem, bool friendly) : base(sandTile.Name + (friendly ? "SandGun" : ""), sandTile.Texture + "Falling") {
        _tile = sandTile;
        _item = sandItem;
        _friendly = friendly;
    }

    public override void SetStaticDefaults() {
        ProjectileID.Sets.FallingBlockDoesNotFallThroughPlatforms[Type] = true;
        ProjectileID.Sets.ForcePlateDetection[Type] = true;
        ProjectileID.Sets.FallingBlockTileItem[Type] = new(_tile.Type, _friendly ? ItemID.None : _item.Type);
    }

    public override void SetDefaults() {
        Projectile.CloneDefaults(_friendly ? ProjectileID.SandBallGun : ProjectileID.SandBallFalling);
    }
}
