using Terraria;

namespace Aequus.Items
{
    public sealed class ShopPrices
    {
        public static int PotionValue => Item.sellPrice(silver: 2);
        public static int EnergySellValue => Item.sellPrice(silver: 10);
        public static int CrabCreviceValue => Item.sellPrice(silver: 25);
        public static int CorruptionWeaponValue => Item.sellPrice(silver: 50);
        public static int CrimsonWeaponValue => Item.sellPrice(silver: 55);
        public static int GlimmerWeaponValue => Item.sellPrice(silver: 75);
        public static int DemonSiegeWeaponValue => Item.sellPrice(silver: 80);
        public static int MemorialistItemBuyValue => Item.buyPrice(gold: 20);
        public static int OmegaStariteDropValue => Item.sellPrice(gold: 4, silver: 50);
        public static int GaleStreamsWeaponValue => Item.sellPrice(gold: 4);
        public static int PostMechsEnergyWeaponValue => Item.sellPrice(gold: 6, silver: 50);
        public static int PillarWeaponValue => Item.sellPrice(gold: 10);
    }
}