using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion {
    public class GravetenderMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}