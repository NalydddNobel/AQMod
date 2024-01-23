namespace Aequus.Core.Utilities;

public static class CommonColor {
    public static readonly Color TILE_FURNITURE = new Color(191, 142, 111, 255);
    public static readonly Color TILE_FURNITURE_LIGHTED = new Color(253, 221, 3, 255);

    public static readonly Color TEXT_BOSS = new Color(175, 75, 255);
    public static readonly Color TEXT_EVENT = new Color(50, 255, 130);
    public static readonly Color TEXT_TOWN_NPC_ARRIVED = new Color(50, 125, 255);
    public static readonly Color TEXT_PREFIX_GOOD = new Color(120, 190, 120);
    public static readonly Color TEXT_PREFIX_BAD = new Color(190, 120, 120);

    public static readonly Color[] COIN_COLORS = new Color[] {
        Colors.CoinCopper,      // 0
        Colors.CoinSilver,      // 1
        Colors.CoinGold,        // 2
        Colors.CoinPlatinum,    // 3
    };
}