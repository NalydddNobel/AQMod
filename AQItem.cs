using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public sealed class AQItem : GlobalItem
    {
        public static class Prices
        {
            public static int PotionValue => Item.sellPrice(silver: 2);
            public static int EnergySellValue => Item.sellPrice(silver: 10);
            public static int EnergyBuyValue => Item.buyPrice(gold: 3);
            public static int CrabsonWeaponValue => Item.sellPrice(silver: 25);
            public static int CorruptionWeaponValue => Item.sellPrice(silver: 50);
            public static int CrimsonWeaponValue => Item.sellPrice(silver: 55);
            public static int GlimmerWeaponValue => Item.sellPrice(silver: 75);
            public static int DemonSiegeWeaponValue => Item.sellPrice(silver: 80);
            public static int OmegaStariteWeaponValue => Item.sellPrice(gold: 4, silver: 50);
            public static int GaleStreamsValue => Item.sellPrice(gold: 4);
            public static int PostMechsEnergyWeaponValue => Item.sellPrice(gold: 6, silver: 50);
            public static int PillarWeaponValue => Item.sellPrice(gold: 10);
        }

        public static class Rarities
        {
            public const int CrabsonWeaponRare = ItemRarityID.Blue;
            public const int StariteWeaponRare = ItemRarityID.Green;
            public const int PetRare = ItemRarityID.Orange;
            public const int GoreNestRare = ItemRarityID.Orange;
            public const int OmegaStariteRare = ItemRarityID.LightRed;
            public const int GaleStreamsRare = ItemRarityID.LightRed;
            public const int PillarWeaponRare = ItemRarityID.Red;
        }
    }
}