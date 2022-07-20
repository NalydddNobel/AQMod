using Aequus.Common.Networking;
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
            SwordSlash = new SoundStyle("Aequus/Sounds/Items/swordslash", 0, 3, SoundType.Sound);
            SwordSwoosh = new SoundStyle("Aequus/Sounds/Items/swordswoosh", 0, 4, SoundType.Sound);
            Thunderclap = new SoundStyle("Aequus/Sounds/RedSprite/thunderclap", 0, 2) { Volume = 0.6f, };
        }

        void ILoadable.Unload()
        {
        }

        public static class NetSoundID
        {
            public const byte ZombieRecruit = 0;
        }

        public static void SendSound(byte type, Vector2? location = null, float? volume = null, float? pitch = null)
        {
            PacketHandler.Send((p) =>
            {
                p.Write(type);
                PacketHandler.FlaggedWrite(location != null, (p) => p.WriteVector2(location.Value), p);
                PacketHandler.FlaggedWrite(volume != null, (p) => p.Write(volume.Value), p);
                PacketHandler.FlaggedWrite(pitch != null, (p) => p.Write(pitch.Value), p);
            }, PacketType.SoundQueue);
        }

        public static void ReceiveSoundQueue(BinaryReader reader)
        {
            byte queueType = reader.ReadByte();
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
                if (queueType == 0)
                {
                    SoundEngine.PlaySound(NecromancyNPC.ZombieRecruitSound, location);
                }
            }
        }
    }
}