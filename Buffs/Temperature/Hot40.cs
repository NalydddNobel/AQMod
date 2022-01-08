using Terraria;
using Terraria.ID;

namespace AQMod.Buffs.Temperature
{
    public class Hot40 : temperatureDebuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.AddBuff(BuffID.OnFire, 1);
        }
    }
}