using System.IO;
using System.Runtime.CompilerServices;
using Terraria.ModLoader.IO;

namespace Aequus.Common.CodeGeneration;

internal static class SourceGeneratorTools {
    // TODO -- Optimize booleans?
    #region Save & Load
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SaveObj<T>(TagCompound tag, string name, T obj) {
        if (obj?.Equals(default(T)) == true) {
            tag[name] = obj;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LoadObj<T>(TagCompound tag, string name, ref T obj) {
        obj = default;
        if (tag.TryGet(name, out T result)) {
            obj = result;
        }
    }
    #endregion

    #region Send
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SendObj(BinaryWriter writer, bool obj) {
        writer.Write(obj);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SendObj(BinaryWriter writer, int obj) {
        writer.Write(obj);
    }
    #endregion

    #region Receive
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReceiveObj(BinaryReader reader, ref bool obj) {
        obj = reader.ReadBoolean();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReceiveObj(BinaryReader reader, ref int obj) {
        obj = reader.ReadInt32();
    }
    #endregion

    public static void ResetObj(ref StatModifier statModifier) {
        statModifier = StatModifier.Default;
    }

    public static void ResetObj<T>(ref T obj) {
        obj = default(T);
    }
}
