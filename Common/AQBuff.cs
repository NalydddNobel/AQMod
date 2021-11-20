using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class AQBuff : GlobalBuff
    {
        public static class Sets
        {
            public static bool[] FoodBuff { get; private set; }

            internal static void Setup()
            {
                FoodBuff = new bool[BuffLoader.BuffCount];
                FoodBuff[BuffID.WellFed] = true;
                FoodBuff[ModContent.BuffType<Buffs.Foods.GrapePhanta>()] = true;
                FoodBuff[ModContent.BuffType<Buffs.Foods.SpicyEel>()] = true;
            }

            internal static void Unload()
            {
            }
        }
    }
}