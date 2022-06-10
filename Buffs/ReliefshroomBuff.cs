using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class ReliefshroomBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}