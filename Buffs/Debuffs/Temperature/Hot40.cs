using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Debuffs.Temperature
{
    public class Hot40 : TemperatureDebuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.AddBuff(BuffID.OnFire, 1);
        }
    }
}