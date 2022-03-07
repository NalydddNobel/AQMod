using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Items.Accessories
{
    public abstract class DyeableAccessory : ModItem
    {
        public byte color;

        public sealed override bool CloneNewInstances => true;

        public Color DyeColor()
        {
            return DyeColor(color);
        }
        public static Color DyeColor(byte dye)
        {
            if (dye == 0)
            {
                return Color.White;
            }
            if (dye == 255)
            {
                return Main.DiscoColor;
            }
            return WorldGen.paintColor(dye);
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["paint"] = color,
            };
        }

        public override void Load(TagCompound tag)
        {
            color = tag.GetByte("paint");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(color);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            color = reader.ReadByte();
        }
    }
}