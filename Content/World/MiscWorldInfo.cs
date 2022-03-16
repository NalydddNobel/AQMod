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

        public static ushort bloodMoonsPrevented;
        public static ushort glimmersPrevented;
        public static ushort eclipsesPrevented;

        public override void Initialize()
        {
            eightBallText = "";
            eightBallExtraText?.Clear();
            eightBallExtraText = new List<string>();

            villagerMoveInAtNight = false;
            villagerLavaImmunity = false;
            terminatorObtained = false;
            bloodMoonDisabled = false;
            glimmerDisabled = false;
            eclipseDisabled = false;

            bloodMoonsPrevented = 0;
            glimmersPrevented = 0;
            eclipsesPrevented = 0;
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
                ["BloodMoon"] = bloodMoonDisabled,
                ["Glimmer"] = glimmerDisabled,
                ["Eclipse"] = eclipseDisabled,

                ["BloodMoonsPrevented"] = bloodMoonsPrevented,
                ["GlimmersPrevented"] = glimmersPrevented,
                ["EclipsesPrevented"] = eclipsesPrevented,
            };
        }

        public override void Load(TagCompound tag)
        {
            eightBallText = tag.GetString("EightBallText");
            eightBallExtraText = (List<string>)tag.GetList<string>("EightBallExtraText");

            villagerMoveInAtNight = tag.GetBool("Stardrop");
            villagerLavaImmunity = tag.GetBool("Terminator");
            terminatorObtained = tag.GetBool("Terminator2");

            bloodMoonDisabled = tag.GetBool("BloodMoon");
            glimmerDisabled = tag.GetBool("Glimmer");
            eclipseDisabled = tag.GetBool("Eclipse");

            bloodMoonsPrevented = tag.Get<ushort>("BloodMoonsPrevented");
            glimmersPrevented = tag.Get<ushort>("GlimmersPrevented");
            eclipsesPrevented = tag.Get<ushort>("EclipsesPrevented");
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
                writer.Write(villagerLavaImmunity);
                writer.Write(terminatorObtained);
                writer.Write(bloodMoonDisabled);
                writer.Write(bloodMoonsPrevented);
                writer.Write(glimmerDisabled);
                writer.Write(glimmersPrevented);
                writer.Write(eclipseDisabled);
                writer.Write(eclipsesPrevented);
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
                villagerLavaImmunity = reader.ReadBoolean();
                terminatorObtained = reader.ReadBoolean();
                bloodMoonDisabled = reader.ReadBoolean();
                bloodMoonsPrevented = reader.ReadUInt16();
                glimmerDisabled = reader.ReadBoolean();
                glimmersPrevented = reader.ReadUInt16();
                eclipseDisabled = reader.ReadBoolean();
                eclipsesPrevented = reader.ReadUInt16();
            }
        }
    }
}