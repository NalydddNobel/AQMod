using Terraria.Audio;
using Terraria.ModLoader;

namespace Aequus
{
    public class SoundHelpers : ILoadable
    {
        public static SoundStyle SwordSlash { get; private set; }
        public static SoundStyle SwordSwoosh { get; private set; }
        public static SoundStyle Thunderclap { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            SwordSlash = new SoundStyle("Aequus/Sounds/Items/swordslash", 0, 3, SoundType.Sound);
            SwordSwoosh = new SoundStyle("Aequus/Sounds/Items/swordswoosh", 0, 4, SoundType.Sound);
            Thunderclap = new SoundStyle("Aequus/Sounds/RedSprite/thunderclap", 0, 2) { Volume = 0.6f, };
        }

        void ILoadable.Unload()
        {
        }
    }
}