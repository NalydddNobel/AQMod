using System;

namespace Aequus.Common;

public class ModRequiredAttribute : Attribute {
    public readonly string ModNeeded;

    public ModRequiredAttribute(string modNeeded) {
        ModNeeded = modNeeded;
    }
}