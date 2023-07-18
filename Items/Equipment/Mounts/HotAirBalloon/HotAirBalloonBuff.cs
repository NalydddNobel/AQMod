using Aequus.Common.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Mounts.HotAirBalloon {
    public class HotAirBalloonBuff : BaseMountBuff {
        public override int MountType => ModContent.MountType<HotAirBalloonMount>();
    }
}