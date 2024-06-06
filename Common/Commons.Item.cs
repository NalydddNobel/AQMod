namespace Aequus.Common;

public partial class Commons {
    public class Rare {
        /// <summary>Rarity shared by early Pre Hardmode materials (Metal Bars, Ores, etc.)</summary>
        public static readonly int MaterialStart = ItemRarityID.White;
        /// <summary>Rarity shared by mid Pre Hardmode materials (Demonite, Crimtane, etc.)</summary>
        public static readonly int MaterialMid = ItemRarityID.Blue;
        /// <summary>Rarity shared by end Pre Hardmode materials (Meteorite, Hellstone, etc.)</summary>
        public static readonly int MaterialEnd = ItemRarityID.Green;
        /// <summary>Rarity shared by early Hardmode materials (Bars, Ores, etc.)</summary>
        public static readonly int MaterialStartHM = ItemRarityID.Orange;
        /// <summary>Rarity shared by Lunar Fragments.</summary>
        public static readonly int MaterialPillars = ItemRarityID.Cyan;

        /// <summary>Rarity shared by items dropped by the King Slime.</summary>
        public static readonly int BossKingSlime = ItemRarityID.Blue;
        /// <summary>Rarity shared by items dropped by the Eye of Cthulhu.</summary>
        public static readonly int BossEyeOfCthulhu = ItemRarityID.Blue;
        /// <summary>Rarity shared by items dropped by the Salamancer.</summary>
        public static readonly int BossSalamancer = ItemRarityID.Blue;
        /// <summary>Rarity shared by items dropped by the Eater of Worlds.</summary>
        public static readonly int BossEaterOfWorlds = ItemRarityID.Blue;
        /// <summary>Rarity shared by items dropped by the Brain of Cthulhu.</summary>
        public static readonly int BossBrainOfCthulhu = ItemRarityID.Blue;
        /// <summary>Rarity shared by items dropped by the Queen Bee.</summary>
        public static readonly int BossQueenBee = ItemRarityID.Orange;
        /// <summary>Rarity shared by items dropped by Deerclops.</summary>
        public static readonly int BossDeerclops = ItemRarityID.Green;
        /// <summary>Rarity shared by items dropped by Skeletron.</summary>
        public static readonly int BossSkeletron = ItemRarityID.Green;
        /// <summary>Rarity shared by items dropped by Ultra Starite.</summary>
        public static readonly int BossUltraStarite = ItemRarityID.Orange;
        /// <summary>Rarity shared by items dropped by Omega Starite.</summary>
        public static readonly int BossOmegaStarite = ItemRarityID.LightRed;
        /// <summary>Rarity shared by items dropped by the Wall of Flesh.</summary>
        public static readonly int BossWallOfFlesh = ItemRarityID.LightRed;
        /// <summary>Rarity shared by items dropped by the Queen Slime.</summary>
        public static readonly int BossQueenSlime = ItemRarityID.Pink;
        /// <summary>Rarity shared by items dropped by the Mechanical Bosses.</summary>
        public static readonly int BossMechanicalBosses = ItemRarityID.Pink;
        /// <summary>Rarity shared by items dropped by the Dust Devil.</summary>
        public static readonly int BossDustDevil = ItemRarityID.Lime;
        /// <summary>Rarity shared by items dropped by Plantera.</summary>
        public static readonly int BossPlantera = ItemRarityID.Yellow;
        /// <summary>Rarity shared by items dropped by Golem.</summary>
        public static readonly int BossGolem = ItemRarityID.Yellow;
        /// <summary>Rarity shared by items dropped by Duke Fishron.</summary>
        public static readonly int BossDukeFishron = ItemRarityID.Yellow;
        /// <summary>Rarity shared by items dropped by the Empress of Light.</summary>
        public static readonly int BossEmpressOfLight = ItemRarityID.Yellow;
        /// <summary>Rarity shared by items dropped by the Lunatic Cultist.</summary>
        public static readonly int BossLunaticCultist = ItemRarityID.Red;
        /// <summary>Rarity shared by items dropped by the Moon Lord.</summary>
        public static readonly int BossMoonLord = ItemRarityID.Red;

        /// <summary>Rarity shared by items found in the Goblin Invasion.</summary>
        public static readonly int EventGoblins = ItemRarityID.Blue;
        /// <summary>Rarity shared by items found in the Glimmer.</summary>
        public static readonly int EventGlimmer = ItemRarityID.Green;
        /// <summary>Rarity shared by items found in the Demon Siege.</summary>
        public static readonly int EventDemonSiege = ItemRarityID.LightRed;
        /// <summary>Rarity shared by items found in the Hardmode Sandstorm (Forbidden set).</summary>
        public static readonly int EventSandstormHM = ItemRarityID.Pink;
        /// <summary>Rarity shared by items found in the Hardmode Blizzard (Frost set).</summary>
        public static readonly int EventBlizzardHM = ItemRarityID.Pink;
        /// <summary>Rarity shared by items found in the Gale Streams.</summary>
        public static readonly int EventGaleStreams = ItemRarityID.Pink;
        /// <summary>Rarity shared by items crafted with Lunar Fragments.</summary>
        public static readonly int EventPillars = ItemRarityID.Red;

        /// <summary>Rarity shared by items from the Ocean and Polluted Ocean.</summary>
        public static readonly int BiomeOcean = ItemRarityID.Blue;
        /// <summary>Rarity shared by items found in the Jungle.</summary>
        public static readonly int BiomeJungle = ItemRarityID.Green;
        /// <summary>Rarity shared by items found in the Dungeon.</summary>
        public static readonly int BiomeDungeon = ItemRarityID.Green;
        /// <summary>Rarity shared by items found in the Hardmode Dungeon.</summary>
        public static readonly int BiomeDungeonHM = ItemRarityID.Yellow;

        /// <summary>Rarity shared by items sold by the Sky Merchant.</summary>
        public static readonly int NPCSkyMerchant = ItemRarityID.Green;

        /// <summary>Rarity shared by items exclusive to the Shimmer.</summary>
        public static readonly int ShimmerExclusives = ItemRarityID.LightPurple;
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