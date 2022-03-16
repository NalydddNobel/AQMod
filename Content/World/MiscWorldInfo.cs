using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.World
{
    public sealed class MiscWorldInfo : ModWorld
    {
        public static string eightBallText;
        public static List<string> eightBallExtraText;

        public static bool villagerMoveInAtNight;
        public static bool villagerLavaImmunity;
        public static bool terminatorObtained;
        public static bool bloodMoonDisabled;
        public static bool glimmerDisabled;
        public static bool eclipseDisabled;

        public override void Initialize()
        {
            eightBallText = "";
            eightBallExtraText?.Clear();
            eightBallExtraText = new List<string>();
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["EightBallText"] = eightBallText ?? "",
                ["EightBallExtraText"] = eightBallExtraText,
                ["Stardrop"] = villagerMoveInAtNight,
                ["Terminator"] = villagerLavaImmunity,
                ["Terminator2"] = terminatorObtained,
            };
        }

        public override void Load(TagCompound tag)
        {
            eightBallText = tag.GetString("EightBallText");
            eightBallExtraText = (List<string>)tag.GetList<string>("EightBallExtraText");
            villagerMoveInAtNight = tag.GetBool("Stardrop");
            villagerLavaImmunity = tag.GetBool("Terminator");
            terminatorObtained = tag.GetBool("Terminator2");
        }

        public override void NetSend(BinaryWriter writer)
        {
            if (string.IsNullOrWhiteSpace(eightBallText))
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(eightBallText);
                writer.Write(eightBallExtraText.Count);
                for (int i = 0; i < eightBallExtraText.Count; i++)
                {
                    writer.Write(eightBallExtraText[i]);
                }
                writer.Write(villagerMoveInAtNight);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                eightBallText = reader.ReadString();
                int count = reader.ReadInt32();
                eightBallExtraText.Clear();
                for (int i = 0; i < count; i++)
                {
                    eightBallExtraText.Add(reader.ReadString());
                }
                villagerMoveInAtNight = reader.ReadBoolean();
            }
        }
    }
}