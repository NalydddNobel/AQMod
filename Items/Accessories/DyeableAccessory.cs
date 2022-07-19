using Aequus.Common;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Accessories
{
    public abstract class DyeableAccessory : ModItem
    {
        public byte color;

        public Color DyeColor()
        {
            return DyeColor(color);
        }
        public static Color DyeColor(byte dye, Player player = null)
        {
            if (dye == 0)
            {
                return Color.White;
            }
            if (dye == 254)
            {
                return Main.teamColor[player != null ? player.team : AequusPlayer.Team];
            }
            if (dye == 255)
            {
                return Main.DiscoColor;
            }
            return WorldGen.paintColor(dye);
        }
        public static Color DyeColor(Item item, Player player)
        {
            return ((item?.ModItem as DyeableAccessory)?.DyeColor()).GetValueOrDefault(Color.White);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["paint"] = color;
        }

        public override void LoadData(TagCompound tag)
        {
            color = tag.GetByte("paint");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(color);
        }

        public override void NetReceive(BinaryReader reader)
        {
            color = reader.ReadByte();
        }

        protected void ColorRecipes<T>() where T : DyeableAccessory
        {
            foreach (byte paint in PaintDatabase.PaintIDs)
            {
                var r = CreateRecipe()
                    .AddIngredient<T>()
                    .AddIngredient(PaintDatabase.PaintToDye[paint])
                    .AddTile(TileID.DyeVat);
                r.createItem.ModItem<T>().color = paint;
                r.Register();
            }
        }
    }
}