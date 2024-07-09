using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader.Core;

namespace AequusRemake.Core.Assets;

public partial class AequusShaders {
    private static bool _loadedAllShaders = false;

    [Conditional("DEBUG")]
    internal static void LoadAllShadersForDragonLensShaderDebuggingSinceItWillThrowAnErrorIfTheyAllAreNotLoaded() {
        if (_loadedAllShaders) {
            return;
        }

        _loadedAllShaders = true;
        if (Main.dedServ) {
            return;
        }

        MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
        TmodFile file = info.Invoke(mod, null) as TmodFile;

        if (file is null) {
            return;
        }

        IEnumerable<TmodFile.FileEntry> shaders = file.Where(n => n.Name.StartsWith("Effects/") && n.Name.Count(a => a == '/') <= 1 && n.Name.EndsWith(".xnb"));

        foreach (TmodFile.FileEntry i in shaders) {
            string name = Path.ChangeExtension(i.Name, "").TrimEnd('.');

            mod.RequestAssetIfExists<Effect>(name, out _);
        }
    }
}
