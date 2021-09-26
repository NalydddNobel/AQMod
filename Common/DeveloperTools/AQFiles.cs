using System.IO;
using Terraria;

namespace AQMod.Common.DeveloperTools
{
    internal class AQFiles
    {
        private static readonly char fs = Path.DirectorySeparatorChar;
        public static readonly string BaseFilePath = Main.SavePath + fs + "Mods" + fs + "AQMod" + fs;
    }
}