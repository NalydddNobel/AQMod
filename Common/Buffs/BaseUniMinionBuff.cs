namespace Aequus.Common.Buffs;

public abstract class BaseUniMinionBuff : BaseMinionBuff {
    protected sealed override System.Int32 MinionProj => CounterProj;
    protected abstract System.Int32 minionProj { get; }
    protected abstract System.Int32 CounterProj { get; }

    public override void SetStaticDefaults() {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref System.Int32 buffIndex) {
        base.Update(player, ref buffIndex);
        if (player.ownedProjectileCounts[MinionProj] > 0 && player.ownedProjectileCounts[minionProj] <= 0) {
            Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.UnitY,
                minionProj, 0, 0f, player.whoAmI);
        }
    }
}