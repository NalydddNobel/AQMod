using System;

namespace Aequus.Core.CodeGeneration;

/// <summary>Parent type which holds attributes which allow generation of Fields and Methods in <see cref="AequusPlayer"/>.</summary>
internal class PlayerGen {
    /// <summary>Generates a field with <paramref name="Name"/> in <see cref="AequusPlayer"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class FieldAttribute<T>(string Name) : Attribute { }

    /// <summary><inheritdoc cref="FieldAttribute{T}"/> This field is reset in <see cref="AequusPlayer.ResetEffects"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ResetFieldAttribute<T>(string Name) : Attribute { }

    /// <summary><inheritdoc cref="FieldAttribute{T}"/> This field is reset in <see cref="AequusPlayer.ResetInfoAccessories"/> and copied to other AequusPlayer instances in <see cref="AequusPlayer.RefreshInfoAccessoriesFromTeamPlayers(Player)"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class InfoFieldAttribute(string Name) : Attribute { }

    /// <summary><inheritdoc cref="FieldAttribute{T}"/> This field is Saved and Loaded through <see cref="AequusPlayer.SaveData(Terraria.ModLoader.IO.TagCompound)"/> and <see cref="AequusPlayer.LoadData(Terraria.ModLoader.IO.TagCompound)"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    internal class SavedFieldAttribute<T>(string Name) : Attribute { }

    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.OnRespawn"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnRespawn : Attribute { }
    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.SetControls"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class SetControls : Attribute { }
    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.ResetEffects"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class ResetEffects : Attribute { }
    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.PostUpdateEquips"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class PostUpdateEquips : Attribute { }
}
