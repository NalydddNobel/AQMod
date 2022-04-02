using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Debuffs.Temperature
{
    public class Cold40 : TemperatureDebuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.AddBuff(BuffID.Chilled, 1);
        }
    }
}