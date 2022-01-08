using AQMod.Common;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.Players
{
    public sealed class BossEncorePlayer : ModPlayer
    {
        public int[] CurrentEncoreKillCount { get; private set; }
        public int[] EncoreBossKillCountRecord { get; private set; }

        public byte[] SerializeEncoreRecords()
        {
            var writer = new BinaryWriter(new MemoryStream(1024));
            if (EncoreBossKillCountRecord == null)
            {
                writer.Write(false);
                return ((MemoryStream)writer.BaseStream).GetBuffer();
            }
            writer.Write(true);
            writer.Write((byte)0);
            for (int i = 0; i < EncoreBossKillCountRecord.Length; i++)
            {
                if (EncoreBossKillCountRecord[i] != 0)
                {
                    writer.Write(true);
                    if (i >= Main.maxNPCTypes)
                    {
                        writer.Write(true);
                        var ModNPC = NPCLoader.GetNPC(i);
                        writer.Write(AQStringCodes.EncodeModName(ModNPC.mod.Name));
                        writer.Write(ModNPC.Name);
                        writer.Write(EncoreBossKillCountRecord[i]);
                    }
                    else
                    {
                        writer.Write(false);
                        writer.Write(i);
                        writer.Write(EncoreBossKillCountRecord[i]);
                    }
                }
            }
            writer.Write(false);
            return ((MemoryStream)writer.BaseStream).GetBuffer();
        }

        public void DeserialzeEncoreRecords(byte[] buffer)
        {
            var reader = new BinaryReader(new MemoryStream(buffer));
            if (!reader.ReadBoolean())
                return;
            byte save = reader.ReadByte();
            while (reader.ReadBoolean())
            {
                if (reader.ReadBoolean())
                {
                    string mod = AQStringCodes.DecodeModName(reader.ReadString());
                    string name = reader.ReadString();
                    int kills = reader.ReadInt32();
                    try
                    {
                        var Mod = ModLoader.GetMod(mod);
                        if (Mod == null)
                            continue;
                        int type = Mod.NPCType(name);
                        if (type != -1)
                            EncoreBossKillCountRecord[type] = kills;
                    }
                    catch
                    {
                    }
                }
                else
                {
                    int type = reader.ReadInt32();
                    int kills = reader.ReadInt32();
                    EncoreBossKillCountRecord[type] = kills;
                }
            }
        }

        public override void Initialize()
        {
            CurrentEncoreKillCount = new int[NPCLoader.NPCCount];
            EncoreBossKillCountRecord = new int[NPCLoader.NPCCount];
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["BossRushRecords"] = SerializeEncoreRecords(),
            };
        }

        public override void Load(TagCompound tag)
        {
            byte[] buffer = tag.GetByteArray("BossRushRecords");
            if (buffer == null || buffer.Length == 0)
                return;
            DeserialzeEncoreRecords(buffer);
        }
    }
}