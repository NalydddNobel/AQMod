using Aequus.Common.Renaming;
using Aequus.Core.Utilities;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common;

public class PreSave : ILoadable {
    public void Load(Mod mod) {
        DetourHelper.AddHook(typeof(Main).Assembly.GetType("Terraria.ModLoader.IO.WorldIO").GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Static), typeof(PreSave).GetMethod(nameof(On_WorldIO_Save), BindingFlags.NonPublic | BindingFlags.Static));
        //WorldIO
    }

    private static void On_WorldIO_Save(Action<string, bool> orig, string path, bool isCloudSave) {
        RenamingSystem.EnsureTagCompoundContents();
        orig(path, isCloudSave);
    }

    public void Unload() {
    }
}