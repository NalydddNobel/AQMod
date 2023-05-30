using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Misc {
    public class RitualBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}