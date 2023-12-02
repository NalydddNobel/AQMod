using System;
using System.Reflection;

namespace Aequus.Core;

/// <summary>
/// Makes the content appear when "Lite" mode is enabled.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class LiteAttribute : Attribute {
    public static bool LiteCheck(Type type) {
        return !Aequus.Lite || type.GetCustomAttribute<LiteAttribute>(inherit: false) != null;
    }

    public static bool LiteCheck(object obj) {
        return LiteCheck(obj.GetType());
    }
}