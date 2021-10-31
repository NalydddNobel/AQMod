using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.MapMarkers
{
    public class LihzahrdMarker : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}