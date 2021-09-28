using AQMod.Assets;
using AQMod.Common.Utilities;
using Terraria.ID;

namespace AQMod.Sounds
{
    public static class AQMusicManager
    {
        public static AQMusicCollector Crabson { get; private set; }
        public static AQMusicCollector GlimmerEvent { get; private set; }
        public static AQMusicCollector OmegaStarite { get; private set; }
        public static AQMusicCollector DemonSiege { get; private set; }

        public static int GetMusic(AQMusicCollector music) => !AssetManager.AssetsLoaded ? 0 : music.GetMusic();

        internal static void Initialize()
        {
            Crabson = new AQMusicCollector(MusicID.Boss1);
            GlimmerEvent = new AQMusicCollector(MusicID.MartianMadness);
            OmegaStarite = new AQMusicCollector(MusicID.Boss4);
            DemonSiege = new AQMusicCollector(MusicID.Boss1);
        }

        internal static void Unload()
        {
            Crabson = null;
            GlimmerEvent = null;
            OmegaStarite = null;
            DemonSiege = null;
        }
    }
}