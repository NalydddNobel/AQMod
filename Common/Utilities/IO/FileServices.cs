using System.IO;

namespace Aequus.Common.Utilities.IO;

public class FileServices {
    public static string TempPath => $"{Main.SavePath}/Mods/Aequus/Temp/".Replace('/', Path.DirectorySeparatorChar);

    public static Stream CreateTempFileStream(string fileName) {
        string path = TempPath;
        Directory.CreateDirectory(path);
        return File.Create($"{path}{fileName}");
    }
}
