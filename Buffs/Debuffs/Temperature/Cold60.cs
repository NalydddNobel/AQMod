using Terraria;
using Terraria.ID;

namespace AQMod.Buffs.Debuffs.Temperature
{
    public class Cold60 : temperatureDebuff
    {
        protected override bool Cold => true;

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.AddBuff(BuffID.Frostburn, 1);
            player.AddBuff(BuffID.Chilled, 1);
        }
    }
}