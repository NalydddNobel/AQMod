using System;

namespace Aequus.Core.CodeGeneration;

internal class PlayerGen {
    /// <summary>Generates a field in <see cref="AequusPlayer"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class FieldAttribute<T>(string Name) : Attribute { }

    /// <summary>Generates a field in <see cref="AequusPlayer"/> which is reset in <see cref="AequusPlayer.ResetEffects"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ResetFieldAttribute<T>(string Name) : Attribute { }

    /// <summary>Generates a field in <see cref="AequusPlayer"/> which is reset in <see cref="AequusPlayer.ResetInfoAccessories"/> and copied to other AequusPlayer instances in <see cref="AequusPlayer.RefreshInfoAccessoriesFromTeamPlayers(Player)"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class InfoFieldAttribute(string Name) : Attribute { }

    /// <summary>Generates a field in <see cref="AequusPlayer"/> which is Saved and Loaded.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    internal class SavedFieldAttribute<T>(string Name) : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnRespawn : Attribute { }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class SetControls : Attribute { }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class ResetEffects : Attribute { }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class PostUpdateEquips : Attribute { }
}
