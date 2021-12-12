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
        public static void PlaySound(Terraria.ModLoader.SoundType type, string name)
        {
            Main.PlaySound((int)type, -1, -1, AQMod.Instance.GetSoundSlot(type, name));
        }
        public static void PlaySound(Terraria.ModLoader.SoundType type, string name, Vector2 position)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, AQMod.Instance.GetSoundSlot(type, name));
        }
        public static void PlaySound(Terraria.ModLoader.SoundType type, string name, Vector2 position, float volume)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, AQMod.Instance.GetSoundSlot(type, name), volume);
        }
        public static void PlaySound(Terraria.ModLoader.SoundType type, string name, Vector2 position, float volume, float pitch)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, AQMod.Instance.GetSoundSlot(type, name), volume, pitch);
        }
        public static void PlaySound(Terraria.ModLoader.SoundType type, string name, float volume)
        {
            Main.PlaySound((int)type, -1, -1, AQMod.Instance.GetSoundSlot(type, name), volume);
        }
        public static void PlaySound(Terraria.ModLoader.SoundType type, string name, float volume, float pitch)
        {
            Main.PlaySound((int)type, -1, -1, AQMod.Instance.GetSoundSlot(type, name), volume, pitch);
        }
    }
}