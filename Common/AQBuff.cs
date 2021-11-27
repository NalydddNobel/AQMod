using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class AQBuff : GlobalBuff
    {
        public static class Sets
        {
            public static bool[] IsFoodBuff { get; private set; }

            internal static void Setup()
            {
                IsFoodBuff = new bool[BuffLoader.BuffCount];
                IsFoodBuff[BuffID.WellFed] = true;
                IsFoodBuff[ModContent.BuffType<Buffs.Foods.GrapePhanta>()] = true;
                IsFoodBuff[ModContent.BuffType<Buffs.Foods.SpicyEel>()] = true;
            }

            internal static void Unload()
            {
            }
        }
    }
}