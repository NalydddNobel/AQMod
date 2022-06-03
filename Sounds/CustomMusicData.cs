using Terraria.ModLoader;

namespace Aequus.Sounds
{
    public struct CustomMusicData
    {
        private string mod;
        private int modMusicID;
        private int vanillaMusicID;

        internal CustomMusicData(int musicID)
        {
            mod = "Aequus";
            vanillaMusicID = musicID;
            modMusicID = -1;
        }

        public int GetID()
        {
            return modMusicID != -1 ? modMusicID : vanillaMusicID;
        }

        public void SetMusic(Mod mod, string musicPath)
        {
            SetMusic(mod, MusicLoader.GetMusicSlot(musicPath));
        }

        public void SetMusic(Mod mod, int musicID)
        {
            this.mod = mod.Name;
            modMusicID = musicID;
        }

        public void ResetMusic(Mod mod)
        {
            if (mod.Name == this.mod)
            {
                this.mod = "Aequus";
                modMusicID = -1;
            }
        }
    }
}