using System;

namespace Aequus.Core.CodeGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class PlayerFieldAttribute(string Name, string Type) : Attribute {
}
