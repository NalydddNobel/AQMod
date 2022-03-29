using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Aequus.Sounds
{
    public static class SoundHelper
    {
        public static void Play(SoundType type, string name)
        {
            if (type != SoundType.Sound)
            {
                name = type.ToString() + "/" + name;
            }
            var slot = SoundLoader.GetLegacySoundSlot(Aequus.Instance.Name + "/Sounds/" + name);
            SoundEngine.PlaySound(slot);
        }
        public static void Play(SoundType type, string name, Vector2 position)
        {
            if (Main.dedServ)
            {
                return;
            }
            if (type != SoundType.Sound)
            {
                name = type.ToString() + "/" + name;
            }
            var slot = SoundLoader.GetLegacySoundSlot(Aequus.Instance.Name + "/Sounds/" + name);
            SoundEngine.PlaySound(slot, position);
        }
        public static void Play(SoundType type, string name, Vector2 position, float volume = 1f, float pitch = 0f)
        {
            if (Main.dedServ)
            {
                return;
            }
            if (type != SoundType.Sound)
            {
                name = type.ToString() + "/" + name;
            }
            var slot = SoundLoader.GetLegacySoundSlot(Aequus.Instance.Name + "/Sounds/" + name);
            SoundEngine.PlaySound(slot.SoundId, (int)position.X, (int)position.Y, slot.Style, volume, pitch);
        }
        public static void Play(this LegacySoundStyle value, Vector2 position, float volume, float pitch)
        {
            SoundEngine.PlaySound(value.SoundId, (int)position.X, (int)position.Y, value.Style, volume, pitch);
        }
        public static void Play(this LegacySoundStyle value, Vector2 position, float volume)
        {
            SoundEngine.PlaySound(value.SoundId, (int)position.X, (int)position.Y, value.Style, volume);
        }
        public static void Play(this LegacySoundStyle value, Vector2 position)
        {
            SoundEngine.PlaySound(value, position);
        }
        public static void Play(this LegacySoundStyle value)
        {
            SoundEngine.PlaySound(value);
        }
    }
}