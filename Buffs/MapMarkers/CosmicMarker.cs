using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.MapMarkers
{
    public class CosmicMarker : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().cosmicMap = true;
        }
    }
}
