using Terraria.ModLoader;

namespace Aequus
{
    public class ConfiguredMusicData
    {
        private string mod;
        private int customMusicID;
        private readonly int music;
        private readonly int otherworldMusic;

        internal ConfiguredMusicData(int music, int otherworldMusic)
        {
            mod = "Aequus";
            this.music = music;
            this.otherworldMusic = otherworldMusic;
            customMusicID = -1;
        }

        /// <summary>
        /// Gets the Music's ID.
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return customMusicID != -1 ? customMusicID : music;
        }

        /// <summary>
        /// Sets the music to use this mod's suggested music. The selection only be reset by your mod.
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="musicPath"></param>
        public void SetMusic(Mod mod, string musicPath)
        {
            SetMusic(mod, MusicLoader.GetMusicSlot(musicPath));
        }

        /// <summary>
        /// Sets the music to use this mod's suggested music. The selection only be reset by your mod.
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="musicID"></param>
        public void SetMusic(Mod mod, int musicID)
        {
            this.mod = mod.Name;
            customMusicID = musicID;
        }

        /// <summary>
        /// Resets the music, unless the mods do not match.
        /// </summary>
        /// <param name="mod"></param>
        public void ResetMusic(Mod mod)
        {
            if (mod == null || mod.Name == this.mod)
            {
                this.mod = "Aequus";
                customMusicID = -1;
            }
        }

        internal void Unload() {
            ResetMusic(null);
        }
    }
}