using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Sounds
{
    public static class AQSound
    {
        public static UnifiedRandom rand { get; internal set; }

        public static class Paths
        {
            public const string NoHit = "Sounds/NPCKilled/NoHit";

            public const string thunderClapSlap = "Sounds/NPCHit/Slap_";
            /// <summary>
            /// Gets a thunder clap slap, randomized.
            /// </summary>
            public static string ThunderClapSlap => thunderClapSlap + rand.Next(2);

            public const string thunderClap = "Sounds/Item/ThunderClap_";
            /// <summary>
            /// Gets a thunder clap, randomized.
            /// </summary>
            public static string ThunderClap => thunderClap + Main.rand.Next(2);

            public const string mysticUmbrellaDestroy = "Sounds/Item/MysticUmbrella/Destroy_";
            /// <summary>
            /// Gets a mystic moon destroy sound, randomized
            /// </summary>
            public static string MysticUmbrellaDestroy => mysticUmbrellaDestroy + Main.rand.Next(4);
            public const string mysticUmbrellaShoot = "Sounds/Item/MysticUmbrella/Shoot_";
            public static string MysticUmbrellaShoot => mysticUmbrellaShoot + Main.rand.Next(3);
            public const string mysticUmbrellaJump = "Sounds/Item/MysticUmbrella/Jump_";
            public static string MysticUmbrellaJump => mysticUmbrellaJump + Main.rand.Next(2);

            public const string nobleMushroomHit = "Sounds/Custom/NobleMushroom/Hit_";
            /// <summary>
            /// Gets a noble mushroom hit sound, randomized.
            /// </summary>
            public static string NobleMushroomHit => nobleMushroomHit + Main.rand.Next(3);
            public const string NobleMushroomDestroy = "Sounds/Custom/NobleMushroom/Destroy";
            public const string MeteorKilled = "Sounds/NPCKilled/Meteor";
            public const string Boowomp = "Sounds/NPCHit/Boowomp";
        }

        private static string qpath(Terraria.ModLoader.SoundType type, string path)
        {
            return "Sounds/" + type.ToString() + "/" + path;
        }

        internal static void Play(this LegacySoundStyle sound)
        {
            Main.PlaySound(sound);
        }
        internal static void Play(this LegacySoundStyle sound, Vector2 position)
        {
            Main.PlaySound(sound, position);
        }
        internal static void Play(this LegacySoundStyle sound, Vector2 position, float volume)
        {
            Main.PlaySound(sound.WithVolume(volume), position);
        }
        internal static void Play(this LegacySoundStyle sound, Vector2 position, float volume, float pitch)
        {
            Main.PlaySound(sound.SoundId, (int)position.X, (int)position.Y, sound.Style, volume, pitch);
        }

        internal static void Play(Terraria.ModLoader.SoundType type, string name)
        {
            Main.PlaySound((int)type, -1, -1, AQMod.GetInstance().GetSoundSlot(type, qpath(type, name)));
        }
        internal static void Play(Terraria.ModLoader.SoundType type, string name, Vector2 position, float volume)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, AQMod.GetInstance().GetSoundSlot(type, qpath(type, name)), volume);
        }
        internal static void Play(Terraria.ModLoader.SoundType type, string name, Vector2 position, float volume, float pitch)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, AQMod.GetInstance().GetSoundSlot(type, qpath(type, name)), volume, pitch);
        }

        internal static void LegacyPlay(Terraria.ModLoader.SoundType type, string name)
        {
            Main.PlaySound((int)type, -1, -1, AQMod.GetInstance().GetSoundSlot(type, name));
        }
        internal static void LegacyPlay(Terraria.ModLoader.SoundType type, string name, Vector2 position)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, AQMod.GetInstance().GetSoundSlot(type, name));
        }
        internal static void LegacyPlay(Terraria.ModLoader.SoundType type, string name, Vector2 position, float volume)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, AQMod.GetInstance().GetSoundSlot(type, name), volume);
        }
        internal static void LegacyPlay(Terraria.ModLoader.SoundType type, string name, Vector2 position, float volume, float pitch)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, AQMod.GetInstance().GetSoundSlot(type, name), volume, pitch);
        }
        internal static void LegacyPlay(Terraria.ModLoader.SoundType type, string name, float volume)
        {
            Main.PlaySound((int)type, -1, -1, AQMod.GetInstance().GetSoundSlot(type, name), volume);
        }
        internal static void LegacyPlay(Terraria.ModLoader.SoundType type, string name, float volume, float pitch)
        {
            Main.PlaySound((int)type, -1, -1, AQMod.GetInstance().GetSoundSlot(type, name), volume, pitch);
        }
        internal static void Play<T>(Terraria.ModLoader.SoundType type) where T : ModSound
        {
            Main.PlaySound((int)type, -1, -1, SoundLoader.GetSoundSlot(type, AQUtils.GetPath<T>()));
        }
        internal static void Play<T>(Terraria.ModLoader.SoundType type, Vector2 position)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, SoundLoader.GetSoundSlot(type, AQUtils.GetPath<T>()));
        }
        internal static void Play<T>(Terraria.ModLoader.SoundType type, Vector2 position, float volume)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, SoundLoader.GetSoundSlot(type, AQUtils.GetPath<T>()), volume);
        }
        internal static void Play<T>(Terraria.ModLoader.SoundType type, Vector2 position, float volume, float pitch)
        {
            Main.PlaySound((int)type, (int)position.X, (int)position.Y, SoundLoader.GetSoundSlot(type, AQUtils.GetPath<T>()), volume, pitch);
        }
        internal static void Play<T>(Terraria.ModLoader.SoundType type, float volume)
        {
            Main.PlaySound((int)type, -1, -1, SoundLoader.GetSoundSlot(type, AQUtils.GetPath<T>()), volume);
        }
        internal static void Play<T>(Terraria.ModLoader.SoundType type, float volume, float pitch)
        {
            Main.PlaySound((int)type, -1, -1, SoundLoader.GetSoundSlot(type, AQUtils.GetPath<T>()), volume, pitch);
        }
    }
}