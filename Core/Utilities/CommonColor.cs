namespace Aequus.Core.Utilities;

public static class CommonColor {
    public static readonly Color MapBathtub = new Color(144, 148, 144);
    public static readonly Color MapWoodFurniture = new Color(191, 142, 111);
    public static readonly Color MapTorch = new Color(253, 221, 3);
    public static readonly Color MapStatue = new Color(144, 148, 144);

    public static readonly Color MapWater = new Color(9, 61, 191);

    /// <summary>Yellow text used by Beds, Clocks, etc.</summary>
    public static readonly Color TextInteractable = new Color(255, 240, 20);
    public static readonly Color TextBoss = new Color(175, 75, 255);
    public static readonly Color TextEvent = new Color(50, 255, 130);
    public static readonly Color TextVillagerHasArrived = new Color(50, 125, 255);
    public static readonly Color TooltipPrefixGood = new Color(120, 190, 120);
    public static readonly Color TooltipPrefixBad = new Color(190, 120, 120);

    public static readonly Color[] CoinColors = [
        Colors.CoinCopper,      // 0
        Colors.CoinSilver,      // 1
        Colors.CoinGold,        // 2
        Colors.CoinPlatinum,    // 3
    ];
}