using Aequus.Content.Necromancy;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
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
            SwordSlash = new SoundStyle("Aequus/Sounds/Items/swordslash", 2, SoundType.Sound);
            SwordSwoosh = new SoundStyle("Aequus/Sounds/Items/swordswoosh", 3, SoundType.Sound);
            Thunderclap = new SoundStyle("Aequus/Sounds/RedSprite/thunderclap", 1) { Volume = 0.6f, };
        }

        void ILoadable.Unload()
        {
        }

        public static void ReadSoundQueue(BinaryReader reader)
        {
            string name = reader.ReadString();
            var location = new Vector2(-1f, -1f);
            float volume = 1f;
            float pitch = 0f;
            if (reader.ReadBoolean())
            {
                location = reader.ReadVector2();
            }
            if (reader.ReadBoolean())
            {
                volume = reader.ReadSingle();
            }
            if (reader.ReadBoolean())
            {
                pitch = reader.ReadSingle();
            }
            if (Main.netMode != NetmodeID.Server)
            {
                if (name == "ZombieRecruit")
                {
                    SoundEngine.PlaySound(NecromancyNPC.ZombieRecruitSound, location);
                }
            }
        }
    }
}