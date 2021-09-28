using AQMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class Bossrush : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().bossrush = true;
            player.aggro += 1000;
        }
    }
}