using Aequus.Common.ID;
using Aequus.Common.Players;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs.Temperature
{
    public abstract class TemperatureDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            int temperature = player.GetModPlayer<AequusPlayer>().temperature;
            if (temperature < 0)
                temperature = -temperature;
            temperature -= 300;
            if (temperature < 0)
            {
                return;
            }
            player.buffTime[buffIndex] = Math.Max(temperature, 2);
        }

        /// <summary>
        /// Gets a Buff ID of a <see cref="TemperatureDebuff"/> using a related <see cref="TemperatureType"/> value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int FromTemperatureType(TemperatureType type)
        {
            switch (type)
            {
                case TemperatureType.Freezing:
                    return ModContent.BuffType<Cold60>();
                case TemperatureType.Colder:
                    return ModContent.BuffType<Cold40>();
                case TemperatureType.Cold:
                    return ModContent.BuffType<Cold20>();
                case TemperatureType.Hot:
                    return ModContent.BuffType<Hot20>();
                case TemperatureType.Hotter:
                    return ModContent.BuffType<Hot40>();
                case TemperatureType.Scorching:
                    return ModContent.BuffType<Hot60>();
            }
            return 0;
        }
    }
}