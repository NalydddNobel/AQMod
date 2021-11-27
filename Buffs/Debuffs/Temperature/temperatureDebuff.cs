using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs.Temperature
{
    public abstract class temperatureDebuff : ModBuff
    {
        protected abstract bool Cold { get; }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = GetType().Namespace.Replace('.', '/') + "/" + (Cold ? "Cold" : "Hot");
            return base.Autoload(ref name, ref texture);
        }

        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            int temperature = player.GetModPlayer<AQPlayer>().temperature;
            if (temperature < 0)
                temperature = -temperature;
            temperature -= 20;
            if (temperature < 0)
            {
                return;
            }
            player.buffTime[buffIndex] = temperature * 32 + 4;
        }
    }
}