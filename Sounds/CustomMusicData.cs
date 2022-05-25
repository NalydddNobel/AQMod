using Terraria.Audio;
using Terraria.ModLoader;

namespace Aequus.Sounds
{
    public struct CustomMusicData
    {
        private string mod;
        private string musicPath;
        private SoundStyle sound;
        private int vanillaMusic;


        internal CustomMusicData(int musicID)
        {
            mod = "Aequus";
            musicPath = "";
            vanillaMusic = musicID;
            sound = default(SoundStyle);
        }

        public int GetID()
        {
            return vanillaMusic;
        }

        public void SetMusic(Mod mod, string musicPath)
        {
            this.mod = mod.Name;
            this.musicPath = musicPath;
            //sound = new SoundStyle(musicPath, SoundType.Music);
        }

        public void ResetMusic(Mod mod)
        {
            if (mod.Name == this.mod)
            {
                this.mod = "Aequus";
                musicPath = "";
            }
        }
    }
}