using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Sounds
{
    public class ConfiguredMusicData
    {
        private string mod;
        private int customMusicID;
        private int baseMusicID;
        private int baseOtherworldyMusicID;

        internal ConfiguredMusicData(int musicID, int otherWorldMusic)
        {
            mod = "Aequus";
            baseMusicID = musicID;
            baseOtherworldyMusicID = otherWorldMusic;
            customMusicID = -1;
        }

        public int GetID()
        {
            return customMusicID != -1 ? customMusicID : !Aequus.otherworldMusic ? baseMusicID : baseOtherworldyMusicID;
        }

        public void SetMusic(Mod mod, string musicPath)
        {
            SetMusic(mod, MusicLoader.GetMusicSlot(musicPath));
        }

        public void SetMusic(Mod mod, int musicID)
        {
            this.mod = mod.Name;
            customMusicID = musicID;
        }

        public void ResetMusic(Mod mod)
        {
            if (mod.Name == this.mod)
            {
                this.mod = "Aequus";
                customMusicID = -1;
            }
        }
    }
}