using Aequus.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Sounds
{
    public sealed class MusicData : ILoadable
    {
        /// <summary>
        /// Used by <see cref="NPCs.Boss.Crabson"/>
        /// </summary>
        public static CustomMusicData CrabsonBoss { get; private set; }
        /// <summary>
        /// Used by <see cref="NPCs.Boss.OmegaStarite"/>
        /// </summary>
        public static CustomMusicData OmegaStariteBoss { get; private set; }
        /// <summary>
        /// Unused
        /// </summary>
        public static CustomMusicData DustDevilBoss { get; private set; }
        /// <summary>
        /// Unused
        /// </summary>
        public static CustomMusicData CrabCreviceBiome { get; private set; }
        /// <summary>
        /// Unused
        /// </summary>
        public static CustomMusicData CrabSeasonEvent { get; private set; }
        /// <summary>
        /// Unused
        /// </summary>
        public static CustomMusicData GlimmerEvent { get; private set; }
        /// <summary>
        /// Used when <see cref="AequusPlayer.eventGaleStreams"/> is true
        /// </summary>
        public static CustomMusicData GaleStreamsEvent { get; private set; }
        public static CustomMusicData DemonSiegeEvent { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            CrabsonBoss = new CustomMusicData(MusicID.Boss1);
            OmegaStariteBoss = new CustomMusicData(MusicID.Boss5);
            DustDevilBoss = new CustomMusicData(MusicID.Boss2);
            CrabCreviceBiome = new CustomMusicData(MusicID.OceanNight);
            CrabSeasonEvent = new CustomMusicData(MusicID.GoblinInvasion);
            GlimmerEvent = new CustomMusicData(MusicID.MartianMadness);
            GaleStreamsEvent = new CustomMusicData(MusicID.WindyDay);
            DemonSiegeEvent = new CustomMusicData(MusicID.Monsoon);
        }

        void ILoadable.Unload()
        {
        }
    }
}
