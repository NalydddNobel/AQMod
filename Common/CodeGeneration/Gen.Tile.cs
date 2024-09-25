using System;

namespace Aequus.Common.CodeGeneration;

/// <summary>Parent type which holds attributes which allow generation of Fields and Methods in <see cref="AequusTile"/>.</summary>
internal partial class Gen {
    /// <summary><see cref="AequusTile.RandomUpdate(int, int, int)"/></summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    internal class AequusTile_RandomUpdate() : Attribute { }
}
