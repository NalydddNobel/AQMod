using Terraria;
using Terraria.ID;

namespace AQMod.Buffs.Debuffs.Temperature
{
    public class Hot40 : temperatureDebuff
    {
        protected override bool Cold => false;

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.AddBuff(BuffID.OnFire, 1);
        }
    }
}