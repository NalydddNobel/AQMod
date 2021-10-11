using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
{
    public sealed class ModifiableMusic
    {
        private readonly int _vanillaMusicID;
        private string _moddedMusicName;
        private Mod _mod;

        internal ModifiableMusic(int vanillaMusicID)
        {
            _vanillaMusicID = vanillaMusicID;
        }

        public bool TryRefresh(Mod mod)
        {
            if (_mod == null)
            {
                return false;
            }
            if (_mod.Name == mod.Name)
            {
                _moddedMusicName = null;
                _mod = null;
                return true;
            }
            return false;
        }

        public bool IsModdedMusic()
        {
            return _moddedMusicName != null;
        }

        /// <summary>
        /// This will set the file path to: <code>MODNAME/Sounds/Music/MUSICNAME</code>
        /// </summary>
        /// <param name="mod">The music mod</param>
        /// <param name="music">The music's name</param>
        public void SetMusic(Mod mod, string music)
        {
            _mod = mod;
            _moddedMusicName = music;
        }

        public int GetMusicID()
        {
            return _moddedMusicName != null ? moddedMusicID() : _vanillaMusicID;
        }

        private int moddedMusicID()
        {
            return _mod.GetSoundSlot(SoundType.Music, "Sounds/Music/" + _moddedMusicName);
        }

        public override string ToString()
        {
            bool modded = IsModdedMusic();
            string output = "modded: " + modded;
            if (modded)
            {
                output += ", mod: " + _mod.Name;
                output += ", modded music name: " + _moddedMusicName;
            }
            output += ", vanilla music id: " + _vanillaMusicID;
            return output;
        }
    }
}