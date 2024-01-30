namespace Aequus.Common.Buffs;

public abstract class BaseMountBuff : ModBuff {
    public abstract System.Int32 MountType { get; }

    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[this.Type] = true;
        Main.buffNoSave[this.Type] = true;
    }

    public override void Update(Player player, ref System.Int32 buffIndex) {
        player.mount.SetMount(MountType, player);
        player.buffTime[buffIndex] = 10;
    }
}