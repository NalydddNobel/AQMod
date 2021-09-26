using AQMod.Common;
using Terraria.ID;

namespace AQMod.Assets.Sounds.Music
{
    public static class AQMusicManager
    {
        public static AQMusicCollector Crabson { get; private set; }
        public static AQMusicCollector GlimmerEvent { get; private set; }
        public static AQMusicCollector OmegaStarite { get; private set; }

        public static int GetMusic(AQMusicCollector music) => !SpriteUtils.AssetsLoaded ? 0 : music.GetMusic();

        internal static void Initialize()
        {
            Crabson = new AQMusicCollector(MusicID.Boss1);
            GlimmerEvent = new AQMusicCollector(MusicID.MartianMadness);
            OmegaStarite = new AQMusicCollector(MusicID.Boss4);
        }

        internal static void Unload()
        {
            Crabson = null;
            GlimmerEvent = null;
            OmegaStarite = null;
        }
    }
}