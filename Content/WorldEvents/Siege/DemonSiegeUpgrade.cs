namespace AQMod.Content.WorldEvents.Siege
{
    public struct DemonSiegeUpgrade
    {
        public readonly int baseItem;
        public readonly int rewardItem;
        public readonly UpgradeProgression progression;
        public readonly ushort upgradeTime;

        public const ushort UpgradeTime_PreHardmode = 5400;

        public DemonSiegeUpgrade(int baseItem, int rewardItem, UpgradeProgression progression, ushort time = UpgradeTime_PreHardmode)
        {
            this.baseItem = baseItem;
            this.rewardItem = rewardItem;
            this.progression = progression;
            upgradeTime = time;
        }
    }
}