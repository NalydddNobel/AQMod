using Terraria.ID;

namespace AQMod.Common.ID
{
    public static class IDEnumerators
    {
        /// <summary>
        /// Gives you a string representation of a certain color
        /// </summary>
        /// <param name="clr"></param>
        /// <returns></returns>
        public static string GetClrName(byte clr) // TODO: ADD THE DEEP COLORS
        {
            switch (clr)
            {
                case PaintID.Red:
                    return "Red";
                case PaintID.Orange:
                    return "Orange";
                case PaintID.Yellow:
                    return "Yellow";
                case PaintID.Lime:
                    return "Lime";
                case PaintID.Green:
                    return "Green";
                case PaintID.Teal:
                    return "Teal";
                case PaintID.Cyan:
                    return "Cyan";
                case PaintID.SkyBlue:
                    return "SkyBlue";
                case PaintID.Blue:
                    return "Blue";
                case PaintID.Purple:
                    return "Purple";
                case PaintID.Violet:
                    return "Violet";
                case PaintID.Pink:
                    return "Pink";
                case PaintID.White:
                    return "White";
                case PaintID.Gray:
                    return "Gray";
                case PaintID.Brown:
                    return "Brown";
                case PaintID.Shadow:
                    return "Shadow";
                case PaintID.Negative:
                    return "Negative";
            }
            return "Unknown";
        }
    }
}