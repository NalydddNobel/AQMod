using Aequus.Items.Misc.Mounts;
using Terraria.ModLoader;

namespace Aequus.Buffs.Mounts {
    public class HotAirBalloonBuff : BaseMountBuff
    {
        public override int MountType => ModContent.MountType<HotAirBalloonMount>();
    }
}