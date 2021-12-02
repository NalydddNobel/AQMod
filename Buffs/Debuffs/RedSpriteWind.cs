using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public class RedSpriteWind : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
        }
    }
}