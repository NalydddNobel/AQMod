using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Aequus.Common.Networking
{
    public static class PacketReader
    {
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
                AequusHelpers.PlaySound(SoundType.Sound, name, location, volume, pitch);
            }
        }
    }
}