namespace Aequus.Core.Utilities;

public static class CommonColor {
    public static readonly Color TILE_FURNITURE = new Color(191, 142, 111);
    public static readonly Color TILE_FURNITURE_LIGHTED = new Color(253, 221, 3);
    public static readonly Color TILE_STATUE = new Color(144, 148, 144);

    public static readonly Color TILE_WATER = new Color(9, 61, 191);

    public static readonly Color TEXT_BOSS = new Color(175, 75, 255);
    public static readonly Color TEXT_EVENT = new Color(50, 255, 130);
    public static readonly Color TEXT_TOWN_NPC_ARRIVED = new Color(50, 125, 255);
    public static readonly Color TEXT_PREFIX_GOOD = new Color(120, 190, 120);
    public static readonly Color TEXT_PREFIX_BAD = new Color(190, 120, 120);

    public static readonly Color[] COIN_TCommonColor = new Color[] {
        TCommonColor.CoinCopper,      // 0
        TCommonColor.CoinSilver,      // 1
        TCommonColor.CoinGold,        // 2
        TCommonColor.CoinPlatinum,    // 3
    };
}