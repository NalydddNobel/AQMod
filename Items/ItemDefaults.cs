using Aequus.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public static class ItemDefaults
    {
        public const int RarityCrabCrevice = ItemRarityID.Blue;
        public const int RarityGlimmer = ItemRarityID.Green;
        public const int RarityDemonSiege = ItemRarityID.Orange;
        public const int RarityOmegaStarite = ItemRarityID.LightRed;
        public const int RarityGaleStreams = ItemRarityID.LightRed;

        internal const int RarityBanner = ItemRarityID.Blue;
        internal const int RarityBossMasks = ItemRarityID.Blue;
        internal const int RarityDemoniteCrimtane = ItemRarityID.Blue;
        internal const int RarityDungeon = ItemRarityID.Green;
        internal const int RarityQueenBee = ItemRarityID.Green;
        internal const int RarityJungle = ItemRarityID.Orange;
        internal const int RarityMolten = ItemRarityID.Orange;
        internal const int RarityPet = ItemRarityID.Orange;
        internal const int RarityWallofFlesh = ItemRarityID.LightRed;
        internal const int RarityPreMechs = ItemRarityID.LightRed;
        internal const int RarityCobaltMythrilAdamantite = ItemRarityID.LightRed;
        internal const int RarityMechs = ItemRarityID.Pink;
        internal const int RarityPlantera = ItemRarityID.Lime;
        internal const int RarityHardmodeDungeon = ItemRarityID.Yellow;
        internal const int RarityMartians = ItemRarityID.Yellow;
        internal const int RarityDukeFishron = ItemRarityID.Yellow;
        internal const int RarityLunaticCultist = ItemRarityID.Cyan;
        internal const int RarityPillars = ItemRarityID.Red;
        internal const int RarityMoonLord = ItemRarityID.Red;

        public static int PotionValue => Item.sellPrice(silver: 2);
        public static int EnergySellValue => Item.sellPrice(silver: 10);
        public static int CrabCreviceValue => Item.sellPrice(silver: 25);
        public static int CorruptionWeaponValue => Item.sellPrice(silver: 50);
        public static int CrimsonWeaponValue => Item.sellPrice(silver: 55);
        public static int GlimmerWeaponValue => Item.sellPrice(silver: 75);
        public static int DemonSiegeWeaponValue => Item.sellPrice(silver: 80);
        public static int MemorialistItemBuyValue => Item.buyPrice(gold: 20);
        public static int OmegaStariteDropValue => Item.sellPrice(gold: 4, silver: 50);
        public static int GaleStreamsValue => Item.sellPrice(gold: 4);
        public static int PostMechsEnergyWeaponValue => Item.sellPrice(gold: 6, silver: 50);
        public static int PillarWeaponValue => Item.sellPrice(gold: 10);

        public static void DefaultToDopeSword<T>(this Item item, int swingTime) where T : DopeSwordBase
        {
            item.useTime = swingTime;
            item.useAnimation = swingTime;
            item.shoot = ModContent.ProjectileType<T>();
            item.shootSpeed = 1f;
            item.DamageType = DamageClass.Melee;
            item.useStyle = ItemUseStyleID.Shoot;
            item.channel = true;
            item.noMelee = true;
            item.noUseGraphic = true;
        }
    }
}