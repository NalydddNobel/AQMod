namespace Aequus.Core;

partial class Aequus {
    internal const bool DEBUG_MODE =
#if DEBUG
            true;
#else
        false;
#endif

    internal static string DEBUG_FILES_PATH => $"{Main.SavePath.Replace("tModLoader-preview", "tModLoader")}";
}
