using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Misc
{
    public class VampirismNight : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}