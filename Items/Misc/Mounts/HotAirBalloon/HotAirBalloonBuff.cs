using Aequus.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Mounts.HotAirBalloon {
    public class HotAirBalloonBuff : BaseMountBuff {
        public override int MountType => ModContent.MountType<HotAirBalloonMount>();
    }
}