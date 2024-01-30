using System;
using System.Reflection;

namespace Aequus.Core;

public class IOHooks : ILoadable {
    public delegate void PreSaveDelegate(Boolean toCloud);
    public static PreSaveDelegate PreSaveWorld { get; internal set; }

    public void Load(Mod mod) {
        HookManager.ApplyAndCacheHook(Aequus.TerrariaAssembly.GetType("Terraria.ModLoader.IO.WorldIO").GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Static), typeof(IOHooks).GetMethod(nameof(On_WorldIO_Save), BindingFlags.NonPublic | BindingFlags.Static));
    }

    private static void On_WorldIO_Save(Action<String, Boolean> orig, String path, Boolean isCloudSave) {
        PreSaveWorld?.Invoke(isCloudSave);
        orig(path, isCloudSave);
    }

    public void Unload() {
        PreSaveWorld = null;
    }
}