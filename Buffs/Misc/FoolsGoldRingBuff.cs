using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Misc
{
    public class FoolsGoldRingBuff : ModBuff
    {
        public override void SetStaticDefaults() {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}