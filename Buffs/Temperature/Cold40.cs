using Terraria;
using Terraria.ID;

namespace AQMod.Buffs.Temperature
{
    public class Cold40 : temperatureDebuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.AddBuff(BuffID.Chilled, 1);
        }
    }
}