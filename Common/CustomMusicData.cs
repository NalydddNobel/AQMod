using Terraria.ModLoader;

namespace Aequus.Common
{
    public struct CustomMusicData
    {
        private string mod;
        private string musicPath;
        private int musicIDCache;
        private readonly int originalMusicID;

        internal CustomMusicData(int musicID)
        {
            mod = "Aequus";
            musicPath = "";
            musicIDCache = musicID;
            originalMusicID = musicID;
        }

        public int GetID()
        {
            return musicIDCache;
        }

        public void SetMusic(Mod mod, string musicPath)
        {
            this.mod = mod.Name;
            this.musicPath = musicPath;
            musicIDCache = SoundLoader.GetSoundSlot(this.musicPath);
        }
        public void ResetMusic(Mod mod)
        {
            if (mod.Name == this.mod)
            {
                this.mod = "Aequus";
                musicPath = "";
                musicIDCache = originalMusicID;
            }
        }
    }
}