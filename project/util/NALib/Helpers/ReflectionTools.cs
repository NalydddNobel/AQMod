using System;

namespace NALib.Helpers;

public sealed class ReflectionTools {
    /// <summary>If the type represents a value type (struct), returns a blank instance of the value type. Otherwise returns null.</summary>
    /// <param name="t"></param>
    /// <returns>The default value of the Type.</returns>
    public static object? GetDefaultValue(Type t) {
        return t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}
