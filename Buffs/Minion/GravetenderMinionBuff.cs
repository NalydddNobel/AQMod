using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion
{
    public class GravetenderMinionBuff : ModBuff
    {
        public override string Texture => Aequus.Buff;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            AequusBuff.CannotClear.Add(Type);
        }
    }
}