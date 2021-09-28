using System;
using Terraria.ModLoader;

namespace AQMod.Sounds
{
    public class AQMusicCollector
    {
        private Func<int> _getMusic;
        private readonly int _vanillaMusicID;
        private string _mod;

        public bool TryRefresh(Mod mod)
        {
            if (_mod == mod.Name)
            {
                _getMusic = null;
                _mod = "Terraria";
                return true;
            }
            return false;
        }

        public void SetMusic(Mod mod, Func<int> getMusic)
        {
            _getMusic = getMusic;
            _mod = mod.Name;
        }

        public int GetMusic()
        {
            return _getMusic != null ? _getMusic() : _vanillaMusicID;
        }

        internal AQMusicCollector(int vanillaMusicID)
        {
            _vanillaMusicID = vanillaMusicID;
            _mod = "Terraria";
        }

        public override string ToString()
        {
            return "current mod: " + _mod + ", current id: " + GetMusic() + ", vanilla id: " + _vanillaMusicID;
        }
    }
}