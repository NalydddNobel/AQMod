using Aequus.Buffs.Minion;
using Aequus.Items.Tools.Mounts;
using Terraria.ModLoader;

namespace Aequus.Buffs.Mounts
{
    public class HotAirBalloonBuff : MountBuffBase
    {
        public override int MountType => ModContent.MountType<HotAirBalloonMount>();
    }
}