using System.IO;
using Terraria;

namespace AQMod.Common.ID
{
    internal class FilePathings
    {
        private static readonly char fs = Path.DirectorySeparatorChar;
        public static readonly string BaseFilePath = Main.SavePath + fs + "Mods" + fs + "AQMod" + fs;
    }
}