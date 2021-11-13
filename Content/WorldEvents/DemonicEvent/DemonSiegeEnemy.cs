using Terraria.ModLoader;

namespace AQMod.Content.WorldEvents.DemonicEvent
{
    public struct DemonSiegeEnemy
    {
        public readonly int type;
        public readonly int spawnWidth;
        public readonly int spawnTime;
        public readonly DemonSiegeUpgradeProgression progression;

        public const int SPAWNTIME_CINDERA = 120;
        public const int SPAWNTIME_PRE_HARDMODE_REGULAR = 150;

        public static DemonSiegeEnemy FromT<T>(DemonSiegeUpgradeProgression progression, int time = SPAWNTIME_PRE_HARDMODE_REGULAR, int width = 20) where T : ModNPC
        {
            return new DemonSiegeEnemy(ModContent.NPCType<T>(), progression, time, width);
        }

        public DemonSiegeEnemy(int type, DemonSiegeUpgradeProgression progression, int time = SPAWNTIME_PRE_HARDMODE_REGULAR, int width = 32)
        {
            this.type = type;
            this.progression = progression;
            spawnTime = time;
            spawnWidth = width;
        }
    }
}