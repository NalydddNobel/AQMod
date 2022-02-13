using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Summon
{
    public class Omori : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.buffNoTimeDisplay[Type] = true;
        }
    }
}