namespace AQMod.Content.World.Events.DemonSiege
{
    public struct DemonSiegeUpgrade
    {
        public readonly int baseItem;
        public readonly int rewardItem;
        public readonly DemonSiegeUpgradeProgression progression;
        public readonly ushort upgradeTime;

        public const ushort UpgradeTime_PreHardmode = 5400;

        public DemonSiegeUpgrade(int baseItem, int rewardItem, DemonSiegeUpgradeProgression progression, ushort time = UpgradeTime_PreHardmode)
        {
            this.baseItem = baseItem;
            this.rewardItem = rewardItem;
            this.progression = progression;
            upgradeTime = time;
        }
    }
}