using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Mounts;

public abstract class BaseMountBuff : ModBuff {
    public abstract int MountType { get; }

    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.mount.SetMount(MountType, player);
        player.buffTime[buffIndex] = 10;
    }
}