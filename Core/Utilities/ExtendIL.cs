using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace Aequus.Core.Utilities;

public static class ExtendIL {
    public static bool MatchPropertySetter(this Instruction instr, Type type, string propertyName) {
        return instr.MatchCall(type, $"set_{propertyName}");
    }
    public static bool MatchPropertyGetter(this Instruction instr, Type type, string propertyName) {
        return instr.MatchCall(type, $"get_{propertyName}");
    }
}
