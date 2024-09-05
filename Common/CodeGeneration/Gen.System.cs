using System;

namespace Aequus.Common.CodeGeneration;

/// <summary>Parent type which holds attributes which allow generation of Fields and Methods in <see cref="AequusPlayer"/>.</summary>
internal partial class Gen {
    /// <summary>Generates a field with <paramref name="Name"/> in <see cref="AequusSystem"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AequusSystem_FieldAttribute<T>(string Name) : Attribute { }

    /// <summary><inheritdoc cref="AequusSystem_FieldAttribute{T}"/> This field is Saved and Loaded through <see cref="AequusSystem.SaveWorldData(Terraria.ModLoader.IO.TagCompound)"/> and <see cref="AequusSystem.LoadWorldData(Terraria.ModLoader.IO.TagCompound)"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    internal class AequusSystem_WorldFieldAttribute<T>(string Name) : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    internal class AequusSystem_PostUpdateWorldAttribute() : Attribute { }
}
