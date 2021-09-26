using AQMod.Common.Skies;
using AQMod.Common.WorldEvents;
using Terraria;

namespace AQMod.Common
{
    public class WorldTimeManager
    {
        public static int dayrateIncrease;

        internal static void Setup()
        {
            On.Terraria.Main.UpdateTime += Main_UpdateTime;
            On.Terraria.Main.UpdateSundial += Main_UpdateSundial;
        }

        private static void Main_UpdateSundial(On.Terraria.Main.orig_UpdateSundial orig)
        {
            orig();
            Main.dayRate += dayrateIncrease;
        }

        private static void Main_UpdateTime(On.Terraria.Main.orig_UpdateTime orig)
        {
            bool settingUpNight = false;
            if (Main.time + Main.dayRate > Main.dayLength)
                settingUpNight = true;
            Main.dayRate += dayrateIncrease;
            orig();
            dayrateIncrease = 0;
            if (settingUpNight)
            {
                GlimmerEvent.UpdateNight();
                GlimmerEventSky.InitNight();
            }
        }

    }
}