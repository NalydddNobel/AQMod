namespace Aequus.Common;

public partial class Commons {
    public class Rare {
        /// <summary>Rarity shared by early Pre Hardmode materials (Metal Bars, Ores, etc.)</summary>
        public const int MaterialStart = 0;
        /// <summary>Rarity shared by mid Pre Hardmode materials (Demonite, Crimtane, etc.)</summary>
        public const int MaterialMid = 1;
        /// <summary>Rarity shared by end Pre Hardmode materials (Meteorite, Hellstone, etc.)</summary>
        public const int MaterialEnd = 2;
        /// <summary>Rarity shared by early Hardmode materials (Bars, Ores, etc.)</summary>
        public const int MaterialStartHM = 3;
        /// <summary>Rarity shared by Lunar Fragments.</summary>
        public const int MaterialPillars = 10;

        /// <summary>Rarity shared by items dropped by the King Slime.</summary>
        public const int BossKingSlime = 1;
        /// <summary>Rarity shared by items dropped by the Eye of Cthulhu.</summary>
        public const int BossEyeOfCthulhu = 1;
        /// <summary>Rarity shared by items dropped by the Salamancer.</summary>
        public const int BossSalamancer = 1;
        /// <summary>Rarity shared by items dropped by the Eater of Worlds.</summary>
        public const int BossEaterOfWorlds = 1;
        /// <summary>Rarity shared by items dropped by the Brain of Cthulhu.</summary>
        public const int BossBrainOfCthulhu = 1;
        /// <summary>Rarity shared by items dropped by the Queen Bee.</summary>
        public const int BossQueenBee = 3;
        /// <summary>Rarity shared by items dropped by Deerclops.</summary>
        public const int BossDeerclops = 2;
        /// <summary>Rarity shared by items dropped by Skeletron.</summary>
        public const int BossSkeletron = 2;
        /// <summary>Rarity shared by items dropped by Ultra Starite.</summary>
        public const int BossUltraStarite = 3;
        /// <summary>Rarity shared by items dropped by Omega Starite.</summary>
        public const int BossOmegaStarite = 5;
        /// <summary>Rarity shared by items dropped by the Wall of Flesh.</summary>
        public const int BossWallOfFlesh = 4;
        /// <summary>Rarity shared by items dropped by the Queen Slime.</summary>
        public const int BossQueenSlime = 5;
        /// <summary>Rarity shared by items dropped by the Mechanical Bosses.</summary>
        public const int BossMechanicalBosses = 5;
        /// <summary>Rarity shared by items dropped by the Dust Devil.</summary>
        public const int BossDustDevil = 7;
        /// <summary>Rarity shared by items dropped by Plantera.</summary>
        public const int BossPlantera = 8;
        /// <summary>Rarity shared by items dropped by Golem.</summary>
        public const int BossGolem = 8;
        /// <summary>Rarity shared by items dropped by Duke Fishron.</summary>
        public const int BossDukeFishron = 8;
        /// <summary>Rarity shared by items dropped by the Empress of Light.</summary>
        public const int BossEmpressOfLight = 8;
        /// <summary>Rarity shared by items dropped by the Lunatic Cultist.</summary>
        public const int BossLunaticCultist = 10;
        /// <summary>Rarity shared by items dropped by the Moon Lord.</summary>
        public const int BossMoonLord = 10;

        /// <summary>Rarity shared by items found in the Goblin Invasion.</summary>
        public const int EventGoblins = 1;
        /// <summary>Rarity shared by items found in the Glimmer.</summary>
        public const int EventGlimmer = 2;
        /// <summary>Rarity shared by items found in the Demon Siege.</summary>
        public const int EventDemonSiege = 4;
        /// <summary>Rarity shared by items found in the Hardmode Sandstorm (Forbidden set).</summary>
        public const int EventSandstormHM = 5;
        /// <summary>Rarity shared by items found in the Hardmode Blizzard (Frost set).</summary>
        public const int EventBlizzardHM = 5;
        /// <summary>Rarity shared by items found in the Gale Streams.</summary>
        public const int EventGaleStreams = 5;
        /// <summary>Rarity shared by items crafted with Lunar Fragments.</summary>
        public const int EventPillars = 10;

        /// <summary>Rarity shared by items from the Ocean and Polluted Ocean.</summary>
        public const int BiomeOcean = 1;
        /// <summary>Rarity shared by items found in the Jungle.</summary>
        public const int BiomeJungle = 2;
        /// <summary>Rarity shared by items found in the Dungeon.</summary>
        public const int BiomeDungeon = 2;
        /// <summary>Rarity shared by items found in the Hardmode Dungeon.</summary>
        public const int BiomeDungeonHM = 8;

        /// <summary>Rarity shared by items sold by the Sky Merchant.</summary>
        public const int NPCSkyMerchant = 2;

        /// <summary>Rarity shared by items exclusive to the Shimmer.</summary>
        public const int ShimmerExclusives = 6;
    }

    public class Cost {
        /// <summary>75 Silver</summary>
        public static readonly int BossSalamancer = Item.sellPrice(gold: 1, silver: 50);
        /// <summary>3 Gold</summary>
        public static readonly int BossUltraStarite = Item.sellPrice(gold: 3);
        /// <summary>3 Gold</summary>
        public static readonly int BossOmegaStarite = Item.sellPrice(gold: 3);
        /// <summary>3 Gold 50 Silver</summary>
        public static readonly int BossDustDevil = Item.sellPrice(gold: 3, silver: 50);
        /// <summary>10 Gold</summary>
        public static readonly int BossMoonLord = Item.sellPrice(gold: 10);

        /// <summary>1 Gold</summary>
        public static readonly int EventGlimmer = Item.sellPrice(gold: 1);
        /// <summary>2 Gold</summary>
        public static readonly int EventDemonSiege = Item.sellPrice(gold: 2);
        /// <summary>2 Gold</summary>
        public static readonly int EventBlizzardHM = Item.sellPrice(gold: 2);
        /// <summary>2 Gold</summary>
        public static readonly int EventSandstormHM = Item.sellPrice(gold: 2);
        /// <summary>3 Gold</summary>
        public static readonly int EventGaleStreams = Item.sellPrice(gold: 3);

        /// <summary>50 Silver</summary>
        public static readonly int BiomeOcean = Item.sellPrice(gold: 1);
        /// <summary>1 Gold 75 Silver</summary>
        public static readonly int BiomeDungeon = Item.sellPrice(gold: 1, silver: 75);
        /// <summary>6 Gold</summary>
        public static readonly int BiomeDungeonHard = Item.sellPrice(gold: 6);

        /// <summary>1 Gold</summary>
        public static readonly int NPCSkyMerchant = Item.sellPrice(gold: 1);
        /// <summary>5 Gold</summary>
        public static readonly int NPCSkyMerchantCustomPrice = Item.buyPrice(gold: 5);
    }
}