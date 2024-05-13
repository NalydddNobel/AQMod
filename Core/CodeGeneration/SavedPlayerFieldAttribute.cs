using System;

namespace Aequus.Core.CodeGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class SavedPlayerFieldAttribute(string Name, string Type) : Attribute {
}
