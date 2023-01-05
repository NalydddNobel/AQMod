using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Buildings
{
    public class BridgeBountyBuff : ModBuff
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