using System;
using System.Reflection;

namespace Aequus.Core.IO;

public class SaveActions : ILoadable {
    public delegate void PreSaveDelegate(bool toCloud);
    public static PreSaveDelegate PreSaveWorld { get; internal set; }

    public void Load(Mod mod) {
        HookManager.ApplyAndCacheHook(Aequus.TerrariaAssembly.GetType("Terraria.ModLoader.IO.WorldIO").GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Static), typeof(SaveActions).GetMethod(nameof(On_WorldIO_Save), BindingFlags.NonPublic | BindingFlags.Static));
    }

    private static void On_WorldIO_Save(Action<string, bool> orig, string path, bool isCloudSave) {
        PreSaveWorld?.Invoke(isCloudSave);
        orig(path, isCloudSave);
    }

    public void Unload() {
        PreSaveWorld = null;
    }
}