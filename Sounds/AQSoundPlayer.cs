using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace AQMod.Sounds
{
    public static class AQSoundPlayer
    {
        public static void PlaySound(this LegacySoundStyle sound)
        {
            Main.PlaySound(sound);
        }
        public static void PlaySound(this LegacySoundStyle sound, Vector2 position)
        {
            Main.PlaySound(sound, position);
        }
        public static void PlaySound(this LegacySoundStyle sound, Vector2 position, float volume)
        {
            Main.PlaySound(sound.WithVolume(volume), position);
        }
        public static void PlaySound(this LegacySoundStyle sound, Vector2 position, float volume, float pitch)
        {
            Main.PlaySound(sound.SoundId, (int)position.X, (int)position.Y, sound.Style, volume, pitch);
        }
    }
}