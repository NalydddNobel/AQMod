using System;

namespace Aequus.Common.CodeGeneration;

/// <summary>Parent type which holds attributes which allow generation of Fields and Methods in <see cref="AequusPlayer"/>.</summary>
internal partial class Gen {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    internal class AequusTile_RandomUpdate() : Attribute { }
}
